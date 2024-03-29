using System.Numerics;
using ModManager.game.user;
using ModManager.network;
using ModManager.utils;
using ProjectCraftNet;

namespace BlackBoxTest;

public class Tests
{
    private Thread _workThread = null!;

    [OneTimeSetUp]
    public void StartServer()
    {
        _workThread = new Thread(() =>
        {
            var testDic = TestContext.CurrentContext.TestDirectory;
            testDic = testDic[..testDic.IndexOf("BlackBoxTest", StringComparison.Ordinal)];
            Program.Main(["-c", Path.Combine(testDic, @"BlackBoxTest\config.toml")]);
        });
        _workThread.Start();
        Thread.Sleep(30000);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _workThread.Join(1000);
        if (_workThread.IsAlive) 
            _workThread.Interrupt();
    }

    private static TcpClient GetClient()
    {
        return new TcpClient("127.0.0.1", 13000);
    }
    
    [Test, Order(1)]
    public async Task LoginAndLogout()
    {
        var tcpClient = GetClient();
        // 成功的场合
        await tcpClient.Connect();
        await tcpClient.Login("kamoeth", "123456");
        Thread.Sleep(1000);
        Assert.That(UserManager.GetOnlineUserCount(), Is.EqualTo(1));
        await tcpClient.Logout();
        Thread.Sleep(1000);
        Assert.That(UserManager.GetOnlineUserCount(), Is.EqualTo(0));
        // 失败的场合
        await tcpClient.Connect();
        await tcpClient.Login("kamoeth", "12341111156");
        Thread.Sleep(1000);
        Assert.That(UserManager.GetOnlineUserCount(), Is.EqualTo(0));
        await tcpClient.Disconnect();
    }
    
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
    
    [Test, Order(3)]
    public async Task Move()
    {
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
    }
}