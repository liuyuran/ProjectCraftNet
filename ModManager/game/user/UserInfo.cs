using System.Numerics;
using ModManager.game.client;
using ModManager.network;

namespace ModManager.game.user;

public class UserInfo
{
    public long UserId { get; init; }
    public string NickName { get; init; }
    public ClientInfo ClientInfo { get; init; }
    public long WorldId { get; set; }
    public Vector3 Position { get; set; }
    public GameMode GameMode { get; set; }
    public bool IsCommandLine { get; set; }
}