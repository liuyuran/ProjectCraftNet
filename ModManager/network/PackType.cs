namespace ModManager.network;

public enum PackType
{
    // 未知消息
    Unknown = -1,
    // 关闭服务器
    Shutdown = 0,
    // 用户连入
    Connect = 1,
    // 方块注册表
    BlockDefine = 2,
    // 聊天数据发送
    Chat = 3,
    // 心跳包
    Ping = 4,
    // 服务器状态
    ServerStatus = 5,
    // 区块数据发送
    Chunk = 100,
    // 方块交互
    ControlBlock = 101,
    // 实体交互
    ControlEntity = 102,
    // 用户移动
    Move = 103,
    // 在线用户列表
    OnlineList = 104,
    // 断开连接
    Disconnect = 105,
    // 方块变化
    BlockChange = 106,
}