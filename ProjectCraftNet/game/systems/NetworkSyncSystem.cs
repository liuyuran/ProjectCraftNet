using System.Numerics;
using Arch.Core;
using Arch.System;
using Google.Protobuf;
using ModManager.config;
using ModManager.network;
using ProjectCraftNet.game.components;

namespace ProjectCraftNet.game.systems;

public class NetworkSyncSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly World _world = world;

    public override void Update(in float deltaTime)
    {
        var sight = ConfigUtil.Instance.GetConfig().Core?.Sight ?? 5;
        var playerQuery = new QueryDescription().WithAll<Player, Position>();
        var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
        var existChunkPosition = new Dictionary<Vector3, ChunkBlockData>();
        _world.Query(in chunkQuery, (ref Position position, ref ChunkBlockData data) => {
            existChunkPosition.Add(position.Val, data);
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
                        var chunkMsg = new ChunkData
                        {
                            X = (long)chunkPosition.X,
                            Y = (long)chunkPosition.Y,
                            Z = (long)chunkPosition.Z
                        };
                        foreach (var item in value.Data)
                        {
                            var itemData = new BlockData
                            {
                                BlockId = item.BlockId
                            };
                            chunkMsg.Blocks.Add(itemData);
                        }
                        NetworkEvents.FireSendEvent(player.SocketId, PackType.Chunk, chunkMsg.ToByteArray());
                    }
                }
            }
        });
    }
}