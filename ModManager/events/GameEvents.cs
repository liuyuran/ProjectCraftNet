using Microsoft.Extensions.Logging;
using ModManager.logger;
using ModManager.user;

namespace ModManager.events;

/// <summary>
/// 表层的事件集合，通常而言mod应当仅监听此类中定义的事件
/// </summary>
public class GameEvents
{
    private ILogger Logger { get; } = SysLogger.GetLogger(typeof(GameEvents));
    public delegate void UserLoginEventHandler(long socketId, UserInfo info);
    public delegate void ChatEventHandler(long socketId, string message);
    public delegate void ArchiveEventHandler();
    // 用户登录事件
    public static event UserLoginEventHandler? UserLoginEvent;
    // 接收聊天栏消息请监听此事件
    public static event ChatEventHandler? ChatEvent;
    // 用户数据归档事件
    public static event ArchiveEventHandler? ArchiveEvent;
    
    public static void FireUserLoginEvent(long socketId, UserInfo info)
    {
        UserLoginEvent?.Invoke(socketId, info);
    }
    
    public static void FireChatEvent(long socketId, string message)
    {
        ChatEvent?.Invoke(socketId, message);
    }
    
    public static void FireArchiveEvent()
    {
        ArchiveEvent?.Invoke();
    }
}