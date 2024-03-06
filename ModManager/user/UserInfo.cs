using System.Numerics;
using ModManager.network;

namespace ModManager.user;

public struct UserInfo
{
    public ClientInfo ClientInfo { get; init; }
    public long WorldId { get; set; }
    public Vector3 Position { get; set; }
}