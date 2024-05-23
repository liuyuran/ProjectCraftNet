using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ModManager.eventBus;
using ModManager.eventBus.events;
using ModManager.game.block;
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
        Logger.LogInformation("{}", Localize(ModId, "Client [{0}:{1}][{2}] connected", clientType, info.SocketId, info.Ip));
        var id = UserManager.UserLogin(connect, info);
        if (id == 0)
        {
            // 登录失败，通知客户端关闭连接
            NetworkEvents.FireSendEvent(info.SocketId, PackType.ShutdownPack, Array.Empty<byte>());
            return;
        }
        EventBus.Trigger(info.SocketId, new UserLoginEvent());
        NetworkEvents.FireSendEvent(info.SocketId, PackType.ConnectPack, []);
        var blockDefineData = new BlockDefine();
        var blockList = BlockManager.GetAllBlockMetas();
        foreach (var block in blockList)
        {
            blockDefineData.Items.Add(new BlockDefineItem
            {
                BlockId = block.BlockId,
                BlockKey = block.BlockKey,
                Material = block.Material
            });
        }
        NetworkEvents.FireSendEvent(info.SocketId, PackType.BlockDefinePack, blockDefineData.ToByteArray());
    }

    private static void DisconnectHandler(ClientInfo info, byte[] data)
    {
        // 断开连接
        EventBus.Trigger(info.SocketId, new UserLogoutEvent());
        UserManager.UserLogout(info.SocketId);
    }
}