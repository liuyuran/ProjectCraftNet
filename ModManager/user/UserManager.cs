using EasilyNET.Security;
using Microsoft.Extensions.Logging;
using ModManager.database;
using ModManager.logger;
using ModManager.network;
using static ModManager.localization.LocalizationManager;
using static ModManager.ModManager;

namespace ModManager.user;

public class UserManager
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(UserManager));
    private readonly Dictionary<ulong, UserInfo> _users = new();
    private static readonly UserManager Instance = new();
    
    public static ulong UserLogin(Connect connect, ClientInfo info) {
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
            ClientInfo = info
        };
        Instance._users.Add(info.SocketId, userInfo);
        Logger.LogInformation("{}", Localize(ModId, "User {0} login", connect.Username));
        return (ulong)id;
    }
    
    public static void UserLogout(ulong socketId)
    {
        Instance._users.Remove(socketId);
    }
    
    public static UserInfo? GetUserInfo(ulong socketId)
    {
        return Instance._users[socketId];
    }
}