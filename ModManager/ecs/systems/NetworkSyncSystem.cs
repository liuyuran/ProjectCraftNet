using System.Numerics;
using Arch.Core;
using Arch.System;
using Google.Protobuf;
using ModManager.config;
using ModManager.ecs.components;
using ModManager.network;

namespace ModManager.ecs.systems;

public class NetworkSyncSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly World _world = world;
    private readonly ChunkData _msgTemplate = new();

    public override void Update(in float deltaTime)
    {
        var sight = ConfigUtil.Instance.GetConfig().Core?.Sight ?? 5;
        var playerQuery = new QueryDescription().WithAll<Player, Position>();
        var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
        var existChunkPosition = new Dictionary<Vector3, byte[]>();
        _world.Query(in chunkQuery, (ref Position position, ref ChunkBlockData data) =>
        {
            _msgTemplate.X = (long)position.Val.X;
            _msgTemplate.Y = (long)position.Val.Y;
            _msgTemplate.Z = (long)position.Val.Z;
            for (var index = 0; index < data.Data.Length; index++)
            {
                var item = data.Data[index];
                var itemData = new BlockData
                {
                    BlockId = item.BlockId
                };
                if (_msgTemplate.Blocks.Count <= index)
                {
                    _msgTemplate.Blocks.Add(itemData);
                }
                else
                {
                    _msgTemplate.Blocks[index] = itemData;
                }
            }

            existChunkPosition.Add(position.Val, _msgTemplate.ToByteArray());
        });
        _world.Query(in playerQuery, (ref Player player, ref Position position) =>
        {
            var chunkPos = position.Val / 100;
            for (var x = -sight; x < sight; x++)
            {
                for (var y = -sight; y < sight; y++)
                {
                    for (var z = -sight; z < sight; z++)
                    {
                        var chunkPosition = new Vector3((float)Math.Round(chunkPos.X) + x,
                            (float)Math.Round(chunkPos.Y) + y,
                            (float)Math.Round(chunkPos.Z) + z);
                        if (!existChunkPosition.TryGetValue(chunkPosition, out var value)) return;
                        NetworkEvents.FireSendEvent(player.SocketId, PackType.Chunk, value);
                    }
                }
            }
        });
    }
}