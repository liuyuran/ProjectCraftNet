using System.Numerics;
using Google.Protobuf;
using ModManager.game.client;
using ModManager.network;
using ModManager.utils;

namespace BlackBoxTest.utils;

public partial class TcpClient
{
    public async Task Login(string username, string password)
    {
        var connectMsg = new Connect
        {
            Username = username,
            Password = password,
            ClientType = (int)ClientType.Normal
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
    
    public async Task DigChunk(IntVector3 chunkPos, IntVector3 blockPos)
    {
        var blockMsg = new PlayerControlBlock
        {
            ChunkX = chunkPos.X,
            ChunkY = chunkPos.Y,
            ChunkZ = chunkPos.Z,
            BlockX = blockPos.X,
            BlockY = blockPos.Y,
            BlockZ = blockPos.Z,
            Type = 1
        };
        await Send((int)PackType.ControlBlock, blockMsg.ToByteArray());
    }
}