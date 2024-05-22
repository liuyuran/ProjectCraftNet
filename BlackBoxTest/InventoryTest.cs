using System.ComponentModel;
using BlackBoxTest.utils;
using ModManager.network;
using ModManager.state;
using ModManager.utils;

namespace BlackBoxTest;

/// <summary>
/// 测试背包交互机制
/// </summary>
public partial class MainTest
{
    [utils.Test(DisplayName = "背包数据拉取测试"), Order(5)]
    public async Task InventoryFetch()
    {
        var tcpClient = GetClient();
        await tcpClient.Connect();
        await tcpClient.Login("kamoeth", "123456");
        var inventoryEvent = new AutoResetEvent(false);
        tcpClient.ReceiveEvent += (type, bytes) =>
        {
            if (type != (int)PackType.InventoryPack) return;
            var data = InventoryMsg.Parser.ParseFrom(bytes);
            // check
            inventoryEvent.Set();
        };
        await tcpClient.FetchInventory();
        Assert.That(inventoryEvent.WaitOne(1000), Is.True);
        await tcpClient.Disconnect();
    }
}