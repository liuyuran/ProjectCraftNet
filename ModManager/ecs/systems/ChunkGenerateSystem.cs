using System.Numerics;
using Arch.Core;
using Arch.System;
using Microsoft.Extensions.Logging;
using ModManager.archive;
using ModManager.config;
using ModManager.ecs.components;
using ModManager.game.generator;
using ModManager.logger;
using ModManager.state;
using ModManager.state.world.chunk;
using ModManager.utils;

namespace ModManager.ecs.systems;

public class ChunkGenerateSystem(World world) : BaseSystem<World, float>(world)
{
    private static readonly ILogger Logger = SysLogger.GetLogger(typeof(ChunkGenerateSystem));
    private readonly World _world = world;
    private readonly HashSet<IntVector3> _existChunkPosition = [];
    private readonly HashSet<IntVector3> _generatingChunk = [];

    public override void Update(in float deltaTime)
    {
        _existChunkPosition.Clear();
        var sight = ConfigUtil.Instance.GetConfig().Core!.Sight;
        var playerQuery = new QueryDescription().WithAll<Player, Position>();
        var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
        _world.Query(in chunkQuery, (ref Position position) => {
            _existChunkPosition.Add(position.ChunkPos);
        });
        _world.Query(in playerQuery, (ref Position position) =>
        {
            var chunkPos = position.ChunkPos;
            GenerateRangeChunkByCenterPosition(chunkPos, sight);
        });
        GenerateRangeChunkByCenterPosition(new IntVector3(0, 0, 0), sight);
        CraftNet.MapInitEvent.Set();
    }
    
    private void GenerateRangeChunkByCenterPosition(IntVector3 centerPosition, int range)
    {
        for (var x = -range; x < range; x++)
        {
            for (var y = -range; y < range; y++)
            {
                for (var z = -range; z < range; z++)
                {
                    var chunkPosition = new IntVector3(centerPosition.X + x, centerPosition.Y + y, centerPosition.Z + z);
                    if (_existChunkPosition.Contains(chunkPosition)) continue;
                    TryGenerateChunkByCenterPosition(chunkPosition);
                }
            }
        }
    }
    
    private void TryGenerateChunkByCenterPosition(IntVector3 centerPosition)
    {
        var chunkPosition = new Position
        {
            ChunkPos = centerPosition,
            InChunkPos = new Vector3()
        };
        var entity = _world.Create(Archetypes.Chunk);
        _world.Set(entity, chunkPosition);
        // 如果未被标记为生成中，尝试从数据库获取存档
        if (!_generatingChunk.Contains(centerPosition))
        {
            var existChunk = ArchiveManager.TryGetChunkData(0, centerPosition);
            if (existChunk != null)
            {
                _world.Set(entity, new ChunkBlockData
                {
                    WorldId = 0,
                    Data = existChunk,
                    Changed = false
                });
                return;
            }
        }
        // 获取不成功则继续生成，如果正在生成中则跳过
        Logger.LogDebug("Generate chunk at [{x}, {y}, {z}]", centerPosition.X, centerPosition.Y, centerPosition.Z);
        var data = ChunkGeneratorManager.GenerateChunkBlockData(0, centerPosition);
        if (data == null)
        {
            _generatingChunk.Add(centerPosition);
            return;
        }

        _generatingChunk.Remove(centerPosition);
        var chunkData = new long[data.Length];
        for (var i = 0; i < data.Length; i++)
        {
            chunkData[i] = data[i].BlockId;
        }
        _world.Set(entity, new ChunkBlockData
        {
            WorldId = 0,
            Data = chunkData,
            Changed = true
        });
        CraftNet.Instance.World.AddChunk(0, new ChunkPos
        {
            X = centerPosition.X,
            Y = centerPosition.Y,
            Z = centerPosition.Z
        }, chunkData);
    }
}