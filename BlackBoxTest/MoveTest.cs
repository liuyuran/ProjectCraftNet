using System.Numerics;
using ModManager.network;
using ModManager.state;
using ModManager.utils;

namespace BlackBoxTest;

/// <summary>
/// 移动相关测试
/// </summary>
public partial class MainTest
{
    [Test, Order(3)]
    public async Task Move()
    {
        CraftNet.MapInitEvent.WaitOne(60000);
        var tcpClient = GetClient();
        var tcpClient2 = GetClient();
        var count = 0;
        tcpClient.ReceiveEvent += (type, bytes) =>
        {
            if (type != (int)PackType.Move) return;
            var data = PlayerMove.Parser.ParseFrom(bytes);
            Assert.Multiple(() =>
            {
                Assert.That(data.ChunkX, Is.EqualTo(1));
                Assert.That(data.ChunkY, Is.EqualTo(0));
                Assert.That(data.ChunkZ, Is.EqualTo(0));
                Assert.That(data.X, Is.EqualTo(0.3f));
                Assert.That(data.Y, Is.EqualTo(0));
                Assert.That(data.Z, Is.EqualTo(0));
                Assert.That(data.Yaw, Is.EqualTo(0));
                Assert.That(data.Pitch, Is.EqualTo(0));
                Assert.That(data.HeadYaw, Is.EqualTo(0));
                Assert.That(data.PlayerId, Is.EqualTo(1));
            });
            count++;
        };  
        tcpClient2.ReceiveEvent += (type, bytes) =>
        {
            if (type != (int)PackType.Move) return;
            var data = PlayerMove.Parser.ParseFrom(bytes);
            Assert.Multiple(() =>
            {
                Assert.That(data.ChunkX, Is.EqualTo(1));
                Assert.That(data.ChunkY, Is.EqualTo(0));
                Assert.That(data.ChunkZ, Is.EqualTo(0));
                Assert.That(data.X, Is.EqualTo(0.3f));
                Assert.That(data.Y, Is.EqualTo(0));
                Assert.That(data.Z, Is.EqualTo(0));
                Assert.That(data.Yaw, Is.EqualTo(0));
                Assert.That(data.Pitch, Is.EqualTo(0));
                Assert.That(data.HeadYaw, Is.EqualTo(0));
                Assert.That(data.PlayerId, Is.EqualTo(1));
            });
            count++;
        };  
        await tcpClient.Connect(54321);
        await tcpClient.Login("kamoeth", "123456");
        await tcpClient2.Connect(54319);
        await tcpClient2.Login("slave", "123456");
        Thread.Sleep(1000);
        await tcpClient.MoveTo(new IntVector3(1, 0, 0), new Vector3(0.3f, 0, 0));
        Thread.Sleep(1000);
        Assert.That(count, Is.EqualTo(2));
        await tcpClient.Disconnect();
        await tcpClient2.Disconnect();
    }
}