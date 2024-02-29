using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ModManager.command;
using ModManager.logger;
using ModManager.network;
using ModManager.user;
using static ModManager.localization.LocalizationManager;
using static ModManager.ModManager;

namespace ModManager.events;

/// <summary>
/// 表层的事件集合，通常而言mod应当仅监听此类中定义的事件
/// </summary>
public class GameEvents
{
    private ILogger Logger { get; } = SysLogger.GetLogger(typeof(GameEvents));
    private static GameEvents Instance { get; } = new();
    
    public delegate void UserLoginEventHandler(ulong socketId, UserInfo info);
    public delegate void ChatEventHandler(ulong socketId, string message);
    
    // 用户登录事件
    public event UserLoginEventHandler? UserLoginEvent;
    // 接收聊天栏消息请监听此事件
    public event ChatEventHandler? ChatEvent;

    private GameEvents()
    {
        // 转发普通聊天栏消息
        ChatEvent += (socketId, message) =>
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            var userInfo = UserManager.GetUserInfo(socketId);
            if (userInfo == null) return;
            var isCommand = CommandManager.TryParseAsCommand(
                (UserInfo) userInfo,
                message
            );
            if (isCommand) return;
            var data = new ChatAndBroadcast
            {
                Msg = message
            };
            var buffer = data.ToByteArray();
            if (buffer == null)
            {
                Logger.LogError("{}", Localize(ModId, "Error when serializing message"));
                return;
            }
            NetworkEvents.FireSendEvent(socketId, PackType.Chat, buffer);
        };
    }
    
    public static void FireUserLoginEvent(ulong socketId, UserInfo info)
    {
        Instance.UserLoginEvent?.Invoke(socketId, info);
    }
    
    public static void FireChatEvent(ulong socketId, string message)
    {
        Instance.ChatEvent?.Invoke(socketId, message);
    }
}