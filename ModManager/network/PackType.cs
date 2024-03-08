namespace ModManager.network;

public enum PackType
{
    // 未知消息
    Unknown = -1,
    // 关闭服务器
    Shutdown = 0,
    // 用户连入
    Connect = 1,
    // 聊天数据发送
    Chat = 2,
    // 心跳包
    Ping = 3,
    // 区块数据发送
    Chunk = 4,
    // 方块交互
    ControlBlock = 5,
    // 实体交互
    ControlEntity = 6,
    // 用户移动
    Move = 7,
    // 服务器状态
    ServerStatus = 8,
    // 在线用户列表
    OnlineList = 9,
    // 断开连接
    Disconnect = 10
}