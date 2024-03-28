using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModManager.eventBus;
using ModManager.eventBus.events;
using ModManager.game.client;
using ModManager.game.user;
using static ModManager.game.localization.LocalizationManager;
using static ModManager.mod.ModManager;

namespace ModManager.network.handlers;

public partial class PackHandlers
{
    private static void ConnectHandler(ClientInfo info, byte[] data)
    {
        // 用户连接
        var connect = Connect.Parser.ParseFrom(data);
        var clientType = (ClientType)connect.ClientType;
        Logger.LogInformation("{}", Localize(ModId, "Client [{0}]{1} connected", clientType, info.Ip));
        var id = UserManager.UserLogin(connect, info);
        if (id == 0)
        {
            // 登录失败，通知客户端关闭连接
            NetworkEvents.FireSendEvent(info.SocketId, PackType.Shutdown, Array.Empty<byte>());
            return;
        }
        NetworkEvents.FireSendEvent(info.SocketId, PackType.Connect, Array.Empty<byte>());
        EventBus.Trigger(info.SocketId, new UserLoginEvent());        
    }

    private static void DisconnectHandler(ClientInfo info, byte[] data)
    {
        // 断开连接
        EventBus.Trigger(info.SocketId, new UserLogoutEvent());
        UserManager.UserLogout(info.SocketId);
    }
}