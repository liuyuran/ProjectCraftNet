using System.Net.Sockets;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ModManager.logger;
using static ModManager.localization.LocalizationManager;
using static ModManager.ModManager;

namespace ModManager.network;

/// <summary>
/// 网络服务抽象类，用于屏蔽服务器层的具体实现
/// </summary>
public static class NetworkEvents
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(NetworkEvents));
    public delegate void SendEventHandler(long socketId, PackType packType, byte[] data);
    public delegate void ReceiveEventHandler(ClientInfo info, PackType packType, byte[] data);
    // 发送数据请调用此事件
    public static event SendEventHandler? SendEvent;
    // 接收数据请监听此事件
    public static event ReceiveEventHandler? ReceiveEvent;
    
    public static void FireSendEvent(long socketId, PackType packType, byte[] data)
    {
        try {
            SendEvent?.Invoke(socketId, packType, data);
        } catch (Exception e) {
            Logger.LogError("{}", Localize(ModId, "Error when sending PackType: {0}, {1}", packType, e.Message));
        }
    }
    
    public static void FireSendTextEvent(long socketId, string text)
    {
        try
        {
            var msg = new ChatAndBroadcast
            {
                Msg = text
            };
            var data = msg.ToByteArray();
            SendEvent?.Invoke(socketId, PackType.Chat, data);
        } catch (Exception e) {
            Logger.LogError("{}", Localize(ModId, "Error when sending PackType: {0}, {1}", PackType.Chat, e.Message));
        }
    }
    
    public static void FireReceiveEvent(long socketId, int packType, byte[] data, Socket socket)
    {
        if (!Enum.IsDefined(typeof(PackType), packType))
        {
            Logger.LogError("{}", Localize(ModId, "Unknown PackType: {0}", packType));
            return;
        }
        if (packType == (int) PackType.Ping)
        {
            // 心跳包不需要处理
            return;
        }
        var info = new ClientInfo
        {
            SocketId = socketId,
            Ip = socket.RemoteEndPoint?.ToString() ?? ""
        };
        try {
            ReceiveEvent?.Invoke(info, (PackType)packType, data);
        } catch (Exception e) {
            Logger.LogError("{}", Localize(ModId, "Error when handling PackType: {0}, {1}", packType, e.Message));
        }
    }
}