using System.Numerics;
using Arch.Core;
using ModManager.game.client;
using ModManager.network;

namespace ModManager.game.user;

public class UserInfo
{
    public long UserId { get; init; }
    public required string NickName { get; init; }
    public required ClientInfo ClientInfo { get; init; }
    public Entity? PlayerEntity { get; set; }
    public long WorldId { get; set; }
    public Vector3 Position { get; set; }
    public GameMode GameMode { get; set; }
    public bool IsCommandLine { get; set; }
}