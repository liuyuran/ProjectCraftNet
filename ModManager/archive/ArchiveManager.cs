using System.Numerics;
using System.Text.Json;
using Arch.Core;
using Microsoft.Extensions.Logging;
using ModManager.config;
using ModManager.database;
using ModManager.ecs.components;
using ModManager.logger;
using ModManager.utils;
using Chunk = ModManager.database.generate.Chunk;

namespace ModManager.archive;

/// <summary>
/// 存档管理器，用来处理数据库存档交互
/// </summary>
public class ArchiveManager
{
    private static readonly ILogger Logger = SysLogger.GetLogger(typeof(ArchiveManager));

    public static void SaveUserInfo(World world)
    {
        using var dbContext = new CoreDbContext();
        using var ts = dbContext.Database.BeginTransaction();
        var playerQuery = new QueryDescription().WithAll<Player, Position>();
        var changed = new List<UserChanged>();
        var chunkSize = ConfigUtil.Instance.GetConfig().Core!.ChunkSize;
        world.Query(in playerQuery, (ref Player player, ref Position position) =>
        {
            changed.Add(new UserChanged
            {
                UserId = player.UserId,
                X = (long) position.ChunkPos.X * chunkSize + (long) Math.Round(position.InChunkPos.X),
                Y = (long) position.ChunkPos.Y * chunkSize + (long) Math.Round(position.InChunkPos.Y),
                Z = (long) position.ChunkPos.Z * chunkSize + (long) Math.Round(position.InChunkPos.Z),
                WorldId = player.WorldId,
                GameMode = (int) player.GameMode
            });
        });
        foreach (var item in changed)
        {
            if (item.UserId == -1) continue;
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

    public static void SaveChunkInfo(World world, long worldId, params IntVector3[] chunkPos)
    {
        using var dbContext = new CoreDbContext();
        var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
        var changed = new List<ChunkChanged>();
        world.Query(in chunkQuery, (ref ChunkBlockData data, ref Position position) =>
        {
            var component = position;
            if (chunkPos.All(pos => pos != component.ChunkPos)) return;
            if (data.WorldId != worldId) return;
            changed.Add(new ChunkChanged
            {
                Pos = component.ChunkPos,
                WorldId = data.WorldId,
                Data = data.Data
            });
        });
        foreach (var item in changed)
        {
            // Define the multi-rule query
            var query = from chunkItem in dbContext.Chunks
                where chunkItem.PosX == item.Pos.X
                      && chunkItem.PosY == item.Pos.Y
                      && chunkItem.PosZ == item.Pos.Z
                      && chunkItem.WorldId == item.WorldId
                select chunkItem;
            var chunk = query.FirstOrDefault();
            var created = chunk == null;
            chunk ??= new Chunk();
            chunk.WorldId = item.WorldId;
            chunk.PosX = item.Pos.X;
            chunk.PosY = item.Pos.Y;
            chunk.PosZ = item.Pos.Z;
            var jsonData = JsonSerializer.Serialize(item.Data);
            chunk.Data = jsonData;
            if (created)
            {
                Logger.LogDebug("Create new chunk: [{}, {}, {}]", chunk.PosX, chunk.PosY, chunk.PosZ);
                dbContext.Chunks.Add(chunk);
            }
            else
            {
                Logger.LogDebug("Update chunk: [{}, {}, {}]", chunk.PosX, chunk.PosY, chunk.PosZ);
                dbContext.Chunks.Update(chunk);
            }
        }
        dbContext.SaveChanges();
    }
    
    public static long[]? TryGetChunkData(long worldId, IntVector3 chunkPos)
    {
        using var dbContext = new CoreDbContext();
        var query = from chunk in dbContext.Chunks
            where chunk.PosX == chunkPos.X
                  && chunk.PosY == chunkPos.Y
                  && chunk.PosZ == chunkPos.Z
                  && chunk.WorldId == worldId
            select chunk;
        var chunkData = query.FirstOrDefault();
        return chunkData == null ? null : JsonSerializer.Deserialize<long[]>(chunkData.Data);
    }
}