namespace BlackBoxTest;

/// <summary>
/// 测试方块交互机制
/// </summary>
public partial class MainTest
{
    [Test, Order(4)]
    public async Task Dig()
    {
        var tcpClient = GetClient();
        await tcpClient.Connect();
        await tcpClient.Login("kamoeth", "123456");
        await tcpClient.Disconnect();
    }
}