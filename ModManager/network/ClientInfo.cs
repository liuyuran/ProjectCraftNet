namespace ModManager.network;

public class ClientInfo
{
    public required long SocketId { get; init; }
    public required string Ip { get; init; }
    public uint Ping { get; set; }
}