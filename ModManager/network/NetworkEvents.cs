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
    public delegate void SendEventHandler(ulong socketId, PackType packType, byte[] data);
    public delegate void ReceiveEventHandler(ulong socketId, PackType packType, byte[] data);
    // 发送数据请调用此事件
    public static event SendEventHandler? SendEvent;
    // 接收数据请监听此事件
    public static event ReceiveEventHandler? ReceiveEvent;
    
    public static void FireReceiveEvent(ulong socketId, int packType, byte[] data)
    {
        if (Enum.IsDefined(typeof(PackType), packType))
        {
            Logger.LogError("{}", Localize(ModId, "Unknown PackType: {0}", packType));
            return;
        }
        if (packType == (int) PackType.Ping)
        {
            // 心跳包不需要处理
            return;
        }
        ReceiveEvent?.Invoke(socketId, (PackType) packType, data);
    }
}