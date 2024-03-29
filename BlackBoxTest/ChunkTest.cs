using ModManager.network;
using ModManager.utils;

namespace BlackBoxTest;

/// <summary>
/// 区块相关测试
/// </summary>
public partial class MainTest
{
    [Test, Order(2)]
    public async Task GetChunk()
    {
        var tcpClient = GetClient();
        var step = 0;
        tcpClient.ReceiveEvent += (type, bytes) =>
        {
            if (type != (int)PackType.Chunk) return;
            var data = ChunkData.Parser.ParseFrom(bytes);
            switch (step)
            {
                case 0:
                    Assert.Multiple(() =>
                    {
                        Assert.That(data.WorldId, Is.EqualTo(0));
                        Assert.That(data.X, Is.EqualTo(1));
                        Assert.That(data.Y, Is.EqualTo(0));
                        Assert.That(data.Z, Is.EqualTo(0));
                        Assert.That(data.Blocks, Is.Not.Empty);
                    });
                    step++;
                    break;
                case 1:
                    Assert.Multiple(() =>
                    {
                        Assert.That(data.WorldId, Is.EqualTo(0));
                        Assert.That(data.X, Is.EqualTo(100));
                        Assert.That(data.Y, Is.EqualTo(0));
                        Assert.That(data.Z, Is.EqualTo(0));
                        Assert.That(data.Blocks, Is.Empty);
                    });
                    step++;
                    break;
            }
        };  
        await tcpClient.Connect();
        await tcpClient.Login("kamoeth", "123456");
        Thread.Sleep(1000);
        await tcpClient.FetchChunk(0, new IntVector3(1, 0, 0));
        Thread.Sleep(1000);
        Assert.That(step, Is.EqualTo(1));
        await tcpClient.FetchChunk(0, new IntVector3(100, 0, 0));
        Thread.Sleep(1000);
        Assert.That(step, Is.EqualTo(2));
        await tcpClient.Disconnect();
    }    
}