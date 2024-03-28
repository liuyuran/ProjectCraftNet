using System.Numerics;
using Google.Protobuf;
using ModManager.network;
using ModManager.utils;

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

    public async Task MoveTo(IntVector3 chunkPos, Vector3 blockPos)
    {
        var moveMsg = new PlayerMove
        {
            ChunkX = chunkPos.X,
            ChunkY = chunkPos.Y,
            ChunkZ = chunkPos.Z,
            X = blockPos.X,
            Y = blockPos.Y,
            Z = blockPos.Z,
            Yaw = 0,
            Pitch = 0,
            HeadYaw = 0,
            PlayerId = 1
        };
        await Send((int)PackType.Move, moveMsg.ToByteArray());
    }
    
    public async Task FetchChunk(long worldId, IntVector3 chunkPos)
    {
        var chunkMsg = new ChunkData
        {
            X = chunkPos.X,
            Y = chunkPos.Y,
            Z = chunkPos.Z,
            WorldId = worldId
        };
        await Send((int)PackType.Chunk, chunkMsg.ToByteArray());
    }
}