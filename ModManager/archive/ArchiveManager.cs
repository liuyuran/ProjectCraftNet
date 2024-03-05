using Arch.Core;
using Microsoft.Extensions.Logging;
using ModManager.database;
using ModManager.logger;

namespace ModManager.archive;

public class ArchiveManager
{
    private static readonly ILogger Logger = SysLogger.GetLogger(typeof(ArchiveManager));

    public static void SaveUserData(World world)
    {
        using var dbContext = new CoreDbContext();
        using var ts = dbContext.Database.BeginTransaction();
        // var playerQuery = new QueryDescription().WithAll<Player, Position>();
        var changed = new List<UserChanged>();
        // _world.Query(in playerQuery, (ref Player player, ref Position position) =>
        // {
        //     changed.Add(new UserChanged
        //     {
        //         UserId = (long)player.UserId,
        //         X = (long)position.Val.X,
        //         Y = (long)position.Val.Y,
        //         Z = (long)position.Val.Z,
        //         WorldId = (int)player.WorldId,
        //     });
        // });
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
        }

        ts.Commit();
    }
}