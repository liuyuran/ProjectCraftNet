namespace ModManager.network;

public enum PackType
{
    // 未知消息
    UnknownPack = -1,
    // 关闭服务器
    ShutdownPack = 0,
    // 用户连入
    ConnectPack = 1,
    // 方块注册表
    BlockDefinePack = 2,
    // 聊天数据发送
    ChatPack = 3,
    // 心跳包
    PingPack = 4,
    // 服务器状态
    ServerStatusPack = 5,
    // 背包信息
    InventoryPack = 6,
    // 区块数据发送
    ChunkPack = 100,
    // 方块交互
    ControlBlockPack = 101,
    // 实体交互
    ControlEntityPack = 102,
    // 用户移动
    MovePack = 103,
    // 在线用户列表
    OnlineListPack = 104,
    // 断开连接
    DisconnectPack = 105,
    // 方块变化
    BlockChangePack = 106,
}