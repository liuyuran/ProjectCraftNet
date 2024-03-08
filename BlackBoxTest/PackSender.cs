using Google.Protobuf;
using ModManager.network;

namespace BlackBoxTest;

public partial class TcpClient
{
    public async Task Login(string username, string password)
    {
        var connectMsg = new Connect
        {
            Username = username,
            Password = password
        };
        await Send((int)PackType.Connect, connectMsg.ToByteArray());
    }
    
    public async Task Logout()
    {
        await Send((int)PackType.Disconnect, Array.Empty<byte>());
        await Disconnect();
    }
}