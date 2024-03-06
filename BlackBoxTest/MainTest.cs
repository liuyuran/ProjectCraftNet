using ProjectCraftNet;

namespace BlackBoxTest;

public class Tests
{
    private readonly TcpClient _tcpClient = new("127.0.0.1", 13000);
    private Thread _workThread;

    [SetUp]
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
    
    [TearDown]
    public void TearDown()
    {
        _workThread.Join(1000);
        if (_workThread.IsAlive) _workThread.Interrupt();
    }
    
    [Test]
    public async Task Test1()
    {
        await _tcpClient.Connect();
    }
}