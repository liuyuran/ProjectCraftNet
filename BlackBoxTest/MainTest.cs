using ModManager.game.user;
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
        Thread.Sleep(1000);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _workThread.Join(1000);
        if (_workThread.IsAlive) _workThread.Interrupt();
    }

    private static TcpClient GetClient()
    {
        return new TcpClient("127.0.0.1", 13000);
    }
    
    [Test]
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
}