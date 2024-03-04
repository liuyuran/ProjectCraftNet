using System.Numerics;
using Arch.Core;
using Arch.System;
using ModManager.generator;
using ProjectCraftNet.game.components;

namespace ProjectCraftNet.game.systems;

public class ChunkGenerateSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly World _world = world;

    public override void Update(in float deltaTime)
    {
        var playerQuery = new QueryDescription().WithAll<Player, Position>();
        var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
        var existChunkPosition = new HashSet<Position>();
        _world.Query(in chunkQuery, (ref Position position) => {
            existChunkPosition.Add(position);
        });
        _world.Query(in playerQuery, (ref Position position) =>
        {
            var chunkPosition = new Position
            {
                Val = new Vector3((float)Math.Round(position.Val.X / 100),
                    (float)Math.Round(position.Val.Y / 100),
                    (float)Math.Round(position.Val.Z / 100))
            };
            if (existChunkPosition.Contains(chunkPosition)) return;
            var entity = _world.Create(Archetypes.Chunk);
            _world.Set(entity, chunkPosition);
            var data = ChunkGeneratorManager.GenerateChunkBlockData(0, chunkPosition.Val);
            var chunkData = new components.BlockData[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                chunkData[i] = new components.BlockData
                {
                    BlockId = data[i].BlockId
                };
            }
            _world.Set(entity, new ChunkBlockData
            {
                Data = chunkData
            });
            existChunkPosition.Add(chunkPosition);
        });
    }
}