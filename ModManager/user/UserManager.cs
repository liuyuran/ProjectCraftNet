using System.Numerics;
using EasilyNET.Security;
using Microsoft.Extensions.Logging;
using ModManager.core;
using ModManager.database;
using ModManager.logger;
using ModManager.network;
using static ModManager.localization.LocalizationManager;
using static ModManager.ModManager;

namespace ModManager.user;

public class UserManager
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(UserManager));
    private readonly Dictionary<long, UserInfo> _users = new();
    private static readonly UserManager Instance = new();
    public static readonly Queue<long> WaitToJoin = new();
    
    public static long UserLogin(Connect connect, ClientInfo info) {
        using var dbContext = new CoreDbContext();
        var user = dbContext.Users.FirstOrDefault(b => b.Username == connect.Username);
        if (user == null)
        {
            Logger.LogDebug("{}", Localize(ModId, "User not found"));
            return 0;
        }
        var byteData = Sm3Crypt.Signature(connect.Password);
        var hexData = BitConverter.ToString(byteData).Replace("-", "");
        if (!user.Password.Equals(hexData, StringComparison.CurrentCultureIgnoreCase))
        {
            Logger.LogDebug("{}", Localize(ModId, "Password not match"));
            return 0;
        }
        var id = user.Id;
        if (id <= 0) return 0;
        var userInfo = new UserInfo {
            ClientInfo = info,
            WorldId = user.WorldId,
            Position = new Vector3(user.PosX, user.PosY, user.PosZ),
            GameMode = (GameMode) user.GameMode
        };
        Instance._users.Add(info.SocketId, userInfo);
        Logger.LogInformation("{}", Localize(ModId, "User {0} login", connect.Username));
        WaitToJoin.Enqueue(info.SocketId);
        return id;
    }
    
    public static void UserLogout(long socketId)
    {
        Instance._users.Remove(socketId);
    }
    
    public static UserInfo? GetUserInfo(long socketId)
    {
        return Instance._users[socketId];
    }
    
    public static int GetOnlineUserCount()
    {
        return Instance._users.Count;
    }
}