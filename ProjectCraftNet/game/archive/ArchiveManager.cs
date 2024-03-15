using System.Numerics;
using System.Text.Json;
using Arch.Core;
using Microsoft.Extensions.Logging;
using ModManager.database;
using ModManager.logger;
using ProjectCraftNet.game.components;
using Chunk = ModManager.database.generate.Chunk;

namespace ProjectCraftNet.game.archive;

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
        world.Query(in playerQuery, (ref Player player, ref Position position) =>
        {
            changed.Add(new UserChanged
            {
                UserId = player.UserId,
                X = (long)position.Val.X,
                Y = (long)position.Val.Y,
                Z = (long)position.Val.Z,
                WorldId = player.WorldId,
                GameMode = (int) player.GameMode
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

    public static void SaveChunkInfo(World world, long worldId, params Vector3[] chunkPos)
    {
        using var dbContext = new CoreDbContext();
        var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
        var changed = new List<ChunkChanged>();
        world.Query(in chunkQuery, (ref ChunkBlockData data, ref Position position) =>
        {
            var component = position;
            if (chunkPos.All(pos => pos != component.Val)) return;
            if (data.WorldId != worldId) return;
            changed.Add(new ChunkChanged
            {
                Pos = component.Val,
                WorldId = data.WorldId,
                Data = data.Data
            });
        });
        foreach (var item in changed)
        {
            // Define the multi-rule query
            var query = from chunkItem in dbContext.Chunks
                where chunkItem.PosX == (int)item.Pos.X
                      && chunkItem.PosY == (int)item.Pos.Y
                      && chunkItem.PosZ == (int)item.Pos.Z
                      && chunkItem.WorldId == item.WorldId
                select chunkItem;
            var chunk = query.FirstOrDefault();
            var created = chunk == null;
            chunk ??= new Chunk();
            chunk.WorldId = item.WorldId;
            chunk.PosX = (int)item.Pos.X;
            chunk.PosY = (int)item.Pos.Y;
            chunk.PosZ = (int)item.Pos.Z;
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
    
    public static components.BlockData[]? TryGetChunkData(long worldId, Vector3 chunkPos)
    {
        using var dbContext = new CoreDbContext();
        var query = from chunk in dbContext.Chunks
            where chunk.PosX == (int)chunkPos.X
                  && chunk.PosY == (int)chunkPos.Y
                  && chunk.PosZ == (int)chunkPos.Z
                  && chunk.WorldId == worldId
            select chunk;
        var chunkData = query.FirstOrDefault();
        return chunkData == null ? null : JsonSerializer.Deserialize<components.BlockData[]>(chunkData.Data);
    }
}