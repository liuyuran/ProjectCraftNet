using BlackBoxTest.utils;
using ModManager.game.user;
using ModManager.network;
using ModManager.state;
using ProjectCraftNet;

namespace BlackBoxTest;

[TestFixture(TestName = "基础测试", Category = "黑盒测试")]
public partial class MainTest
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
        CraftNet.MapInitEvent.WaitOne(60000);
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

    /// <summary>
    /// 测试登入登出机制
    /// </summary>
    [utils.Test(DisplayName = "登入登出测试"), Order(1)]
    [TestCase("kamoeth", "123456", false, TestName = "成功")]
    [TestCase("kamoeth", "fake", true, TestName = "失败")]
    public async Task LoginAndLogout(string username, string password, bool shouldFail)
    {
        var loginEvent = new AutoResetEvent(false);
        var blockDefineEvent = new AutoResetEvent(false);
        var tcpClient = GetClient();
        // 成功的场合
        tcpClient.ReceiveEvent += (type, bytes) =>
        {
            switch (type)
            {
                case (int)PackType.Shutdown:
                    if (shouldFail) loginEvent.Set();
                    break;
                case (int)PackType.Connect:
                    if (!shouldFail) loginEvent.Set();
                    break;
                case (int)PackType.BlockDefine:
                    if (shouldFail) break;
                    var data = BlockDefine.Parser.ParseFrom(bytes);
                    Assert.That(data.Items, Is.Not.Empty);
                    blockDefineEvent.Set();
                    break;
            }
        };
        await tcpClient.Connect();
        await tcpClient.Login(username, password);
        Assert.Multiple(() =>
        {
            Assert.That(loginEvent.WaitOne(1000), Is.True);
            if (!shouldFail) Assert.That(blockDefineEvent.WaitOne(1000), Is.True);
            Assert.That(UserManager.GetOnlineUserCount(), Is.EqualTo(shouldFail ? 0 : 1));
        });
        if (!shouldFail)
        {
            await tcpClient.Logout();
            Thread.Sleep(1000);
            Assert.That(UserManager.GetOnlineUserCount(), Is.EqualTo(0));
        }

        await tcpClient.Disconnect();
    }
}