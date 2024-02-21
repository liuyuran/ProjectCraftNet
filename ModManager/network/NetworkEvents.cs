namespace ModManager.network;

/// <summary>
/// 网络服务抽象类，用于屏蔽服务器层的具体实现
/// </summary>
public static class NetworkEvents
{
    public delegate void SendEventHandler(ulong socketId, int packType, byte[] data);
    public delegate void ReceiveEventHandler(ulong socketId, int packType, byte[] data);
    // 发送数据请调用此事件
    public static event SendEventHandler? SendEvent;
    // 接收数据请监听此事件
    public static event ReceiveEventHandler? ReceiveEvent;
    
    public static void FireReceiveEvent(ulong socketId, int packType, byte[] data)
    {
        ReceiveEvent?.Invoke(socketId, packType, data);
    }
}