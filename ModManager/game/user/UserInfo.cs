using Arch.Core;
using ModManager.game.client;
using ModManager.network;
using ModManager.utils;

namespace ModManager.game.user;

public class UserInfo
{
    public long UserId { get; init; }
    public required string NickName { get; init; }
    public required ClientInfo ClientInfo { get; init; }
    public Entity? PlayerEntity { get; set; }
    public long WorldId { get; init; } // 以后需要改为可变
    public LongVector3 Position { get; set; }
    public GameMode GameMode { get; init; } // 以后需要改为可变
    public bool IsCommandLine { get; set; }
}