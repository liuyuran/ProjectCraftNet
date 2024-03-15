using System.Numerics;
using ModManager.core;
using ModManager.network;

namespace ModManager.user;

public struct UserInfo
{
    public long UserId { get; init; }
    public ClientInfo ClientInfo { get; init; }
    public long WorldId { get; set; }
    public Vector3 Position { get; set; }
    public GameMode GameMode { get; set; }
    public bool IsCommandLine { get; set; }
}