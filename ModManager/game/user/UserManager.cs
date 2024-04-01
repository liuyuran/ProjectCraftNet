using System.Numerics;
using Arch.Core;
using EasilyNET.Security;
using Microsoft.Extensions.Logging;
using ModManager.database;
using ModManager.eventBus;
using ModManager.eventBus.events;
using ModManager.game.client;
using ModManager.logger;
using ModManager.network;
using ModManager.utils;
using static ModManager.game.localization.LocalizationManager;
using static ModManager.mod.ModManager;

namespace ModManager.game.user;

public class UserManager
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(UserManager));
    private readonly Dictionary<long, UserInfo> _users = new();
    private static readonly UserManager Instance = new();
    public static readonly Queue<long> WaitToJoin = new();
    public static readonly Queue<Entity> WaitToLeave = new();

    private UserManager()
    {
        EventBus.Subscribe<UserLogoutEvent>(evt =>
        {
            UserLogout(evt.SocketId);
            return false;
        });
    }

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
            UserId = user.Id,
            NickName = user.Nickname,
            ClientInfo = info,
            WorldId = user.WorldId,
            Position = new LongVector3(user.PosX, user.PosY, user.PosZ),
            GameMode = (GameMode) user.GameMode,
            PlayerEntity = null
        };
        Instance._users.Add(info.SocketId, userInfo);
        Logger.LogInformation("{}", Localize(ModId, "User {0} login", connect.Username));
        if (connect.ClientType == (int)ClientType.CommandLine)
        {
            userInfo.IsCommandLine = true;
        }
        else
        {
            userInfo.IsCommandLine = false;
            WaitToJoin.Enqueue(info.SocketId);
        }
        return id;
    }
    
    public static void UserLogout(long socketId)
    {
        if (!Instance._users.TryGetValue(socketId, out var value)) return;
        if (value.PlayerEntity != null)
        {
            WaitToLeave.Enqueue(value.PlayerEntity.Value);
        }
        Instance._users.Remove(socketId);
    }
    
    public static UserInfo? GetUserInfo(long socketId)
    {
        return Instance._users!.GetValueOrDefault(socketId, null);
    }
    
    public static int GetOnlineUserCount()
    {
        return Instance._users.Count;
    }
    
    public static void SetClientPing(long socketId, uint ping)
    {
        var userInfo = GetUserInfo(socketId);
        if (userInfo == null) return;
        userInfo.ClientInfo.Ping = ping;
    }

    public static List<UserInfo> GetOnlineUsers()
    {
        return Instance._users.Values.ToList();
    }
}