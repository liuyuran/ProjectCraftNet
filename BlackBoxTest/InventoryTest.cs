using ModManager.network;

namespace BlackBoxTest;

/// <summary>
/// 测试背包交互机制
/// </summary>
public partial class MainTest
{
    [TestCase, Order(5)]
    public async Task InventoryFetch()
    {
        var tcpClient = GetClient();
        await tcpClient.Connect();
        await tcpClient.Login("kamoeth", "123456");
        var loginEvent = new AutoResetEvent(false);
        var inventoryEvent = new AutoResetEvent(false);
        tcpClient.ReceiveEvent += (type, bytes) =>
        {
            if (type == (int)PackType.ConnectPack) loginEvent.Set();
            if (type != (int)PackType.InventoryPack) return;
            // var data = InventoryMsg.Parser.ParseFrom(bytes);
            // check
            inventoryEvent.Set();
        };
        Assert.That(loginEvent.WaitOne(1000), Is.True);
        await tcpClient.FetchInventory();
        Assert.That(inventoryEvent.WaitOne(1000), Is.True);
        await tcpClient.Disconnect();
    }
}