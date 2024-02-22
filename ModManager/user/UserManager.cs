using Microsoft.Extensions.Logging;
using ModManager.logger;
using ModManager.network;
using ProjectCraftNet.game.user;

namespace ModManager.user;

public class UserManager
{
    private ILogger Logger { get; } = SysLogger.GetLogger(typeof(UserManager));
    private readonly Dictionary<ulong, UserInfo> _users = new();
    private ulong _nextId = 1;
    private static readonly UserManager Instance = new();
    
    public static ulong UserLogin(Connect connect, ClientInfo info)
    {
        var id = Instance._nextId++;
        Instance._users.Add(id, new UserInfo
        {
            ClientInfo = info
        });
        return id;
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