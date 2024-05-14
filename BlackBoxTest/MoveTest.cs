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
    [utils.Test(DisplayName = "移动测试"), Order(3)]
    public async Task Move()
    {
        var loginAEvent = new AutoResetEvent(false);
        var loginBEvent = new AutoResetEvent(false);
        var receiveAEvent = new AutoResetEvent(false);
        var receiveBEvent = new AutoResetEvent(false);
        var tcpClient = GetClient();
        var tcpClient2 = GetClient();
        tcpClient.ReceiveEvent += (type, bytes) =>
        {
            if (type == (int)PackType.ConnectPack) loginAEvent.Set();
            if (type != (int)PackType.MovePack) return;
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
            receiveAEvent.Set();
        };  
        tcpClient2.ReceiveEvent += (type, bytes) =>
        {
            if (type == (int)PackType.ConnectPack) loginBEvent.Set();
            if (type != (int)PackType.MovePack) return;
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
            receiveBEvent.Set();
        };  
        await tcpClient.Connect(54321);
        await tcpClient.Login("kamoeth", "123456");
        Assert.That(loginAEvent.WaitOne(1000), Is.True);
        await tcpClient2.Connect(54319);
        await tcpClient2.Login("slave", "123456");
        Assert.That(loginBEvent.WaitOne(1000), Is.True);
        await tcpClient.MoveTo(new IntVector3(1, 0, 0), new Vector3(0.3f, 0, 0));
        Assert.Multiple(() =>
        {
            Assert.That(receiveAEvent.WaitOne(1000), Is.True);
            Assert.That(receiveBEvent.WaitOne(1000), Is.True);
        });
        await tcpClient.Disconnect();
        await tcpClient2.Disconnect();
    }
}