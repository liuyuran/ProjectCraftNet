using System.Numerics;
using Arch.Core;
using Microsoft.Extensions.Logging;
using ModManager.database;
using ModManager.logger;
using ProjectCraftNet.game.components;

namespace ProjectCraftNet.game.archive;

public class ArchiveManager
{
    private static readonly ILogger Logger = SysLogger.GetLogger(typeof(ArchiveManager));

    public static void SaveUserInfo(World world)
    {
        using var dbContext = new CoreDbContext();
        using var ts = dbContext.Database.BeginTransaction();
        var playerQuery = new QueryDescription().WithAll<Player, Position>();
        var changed = new List<UserChanged>();
        world.Query(in playerQuery, (ref Player player, ref Position position) =>
        {
            changed.Add(new UserChanged
            {
                UserId = player.UserId,
                X = (long)position.Val.X,
                Y = (long)position.Val.Y,
                Z = (long)position.Val.Z,
                WorldId = player.WorldId,
                GameMode = player.GameMode
            });
        });
        foreach (var item in changed)
        {
            var user = dbContext.Users.Find(item.UserId);
            if (user == null)
            {
                Logger.LogError("User not found: {}", item.UserId);
                continue;
            }

            user.PosX = item.X;
            user.PosY = item.Y;
            user.PosZ = item.Z;
            user.WorldId = item.WorldId;
            user.GameMode = item.GameMode;
        }

        ts.Commit();
    }

    public static void SaveChunkInfo(World world, params Vector3[] chunkPos)
    {
        using var dbContext = new CoreDbContext();
        using var ts = dbContext.Database.BeginTransaction();
        var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
        var changed = new List<ChunkChanged>();
        world.Query(in chunkQuery, (ref ChunkBlockData data, ref Position position) =>
        {
            var component = position;
            if (chunkPos.All(pos => pos != component.Val)) return;
            changed.Add(new ChunkChanged
            {
                Pos = component.Val,
                WorldId = data.WorldId,
                Data = data.Data
            });
        });
        foreach (var item in changed)
        {
            var chunk = dbContext.Chunks.Find(item.Pos);
            if (chunk == null)
            {
                Logger.LogError("Chunk not found: {}", item.Pos);
                continue;
            }

            chunk.WorldId = item.WorldId;
            // chunk.Data = item.Data;
            chunk.Data = "";
        }
        ts.Commit();
    }
}