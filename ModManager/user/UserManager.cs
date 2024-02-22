using EasilyNET.Security;
using Microsoft.Extensions.Logging;
using ModManager.database;
using ModManager.logger;
using ModManager.network;

namespace ModManager.user;

public class UserManager
{
    private ILogger Logger { get; } = SysLogger.GetLogger(typeof(UserManager));
    private readonly Dictionary<ulong, UserInfo> _users = new();
    private static readonly UserManager Instance = new();
    
    public static ulong UserLogin(Connect connect, ClientInfo info) {
        using var dbContext = new CoreDbContext();
        var user = dbContext.Users.FirstOrDefault(b => b.Username == connect.Username);
        if (user == null) return 0;
        var byteData = Sm3Crypt.Signature(connect.Password);
        var hexData = BitConverter.ToString(byteData).Replace("-", "");
        if (user.Password != hexData) return 0;
        var id = user.Id;
        if (id <= 0) return 0;
        var userInfo = new UserInfo {
            ClientInfo = info
        };
        Instance._users.Add(info.SocketId, userInfo);
        return (ulong)id;
    }
    
    public static void UserLogout(ulong id)
    {
        Instance._users.Remove(id);
    }
    
    public static UserInfo GetUserInfo(ulong id)
    {
        return Instance._users[id];
    }
}