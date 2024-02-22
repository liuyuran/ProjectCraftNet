using ModManager.network;

namespace ProjectCraftNet.game.user;

public struct UserInfo
{
    public ClientInfo ClientInfo { get; set; }
    public ulong SocketId { get; set; }
}