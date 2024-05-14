using System.ComponentModel;
using BlackBoxTest.utils;
using ModManager.network;
using ModManager.state;
using ModManager.utils;

namespace BlackBoxTest;

/// <summary>
/// 测试方块交互机制
/// </summary>
public partial class MainTest
{
    private int _digStep;
    private IntVector3? _digTarget;

    [utils.Test(DisplayName = "方块操作测试"), Order(4)]
    public async Task Dig()
    {
        var tcpClient = GetClient();
        await tcpClient.Connect();
        await tcpClient.Login("kamoeth", "123456");
        var chunkEvent = new AutoResetEvent(false);
        var blockChangeEvent = new AutoResetEvent(false);
        var chunk = Array.Empty<long>();
        tcpClient.ReceiveEvent += (type, bytes) =>
        {
            if (type != (int)PackType.ChunkPack && type != (int)PackType.BlockChangePack) return;
            if (type == (int)PackType.BlockChangePack)
            {
                var data = BlockChange.Parser.ParseFrom(bytes);
                Assert.Multiple(() =>
                {
                    Assert.That(data.ChunkX, Is.EqualTo(0));
                    Assert.That(data.ChunkY, Is.EqualTo(0));
                    Assert.That(data.ChunkZ, Is.EqualTo(0));
                    Assert.That(data.BlockX, Is.EqualTo(_digTarget?.X));
                    Assert.That(data.BlockY, Is.EqualTo(_digTarget?.Y));
                    Assert.That(data.BlockZ, Is.EqualTo(_digTarget?.Z));
                    Assert.That(data.BlockId, Is.EqualTo(0));
                    Assert.That(data.SubId, Is.EqualTo(0));
                    Assert.That(data.ChangeType, Is.EqualTo(0));
                });
                blockChangeEvent.Set();
                return;
            }
            switch (_digStep)
            {
                case 0:
                {
                    var chunkPack = ChunkData.Parser.ParseFrom(bytes);
                    chunk = new long[chunkPack.Blocks.Count];
                    for (var index = 0; index < chunkPack.Blocks.Count; index++)
                    {
                        var block = chunkPack.Blocks[index];
                        chunk[index] = block.BlockId;
                    }

                    chunkEvent.Set();
                    break;
                }
                case 1:
                {
                    var chunkPack = ChunkData.Parser.ParseFrom(bytes);
                    var newChunk = new long[chunkPack.Blocks.Count];
                    Assert.That(_digTarget, Is.Not.Null);
                    var changePoint = _digTarget?.X + _digTarget?.Y * 16 + _digTarget?.Z * 256 ?? 0;
                    for (var index = 0; index < chunkPack.Blocks.Count; index++)
                    {
                        var block = chunkPack.Blocks[index];
                        newChunk[index] = block.BlockId;
                    }
                    for (var i = 0; i < newChunk.Length; i++)
                    {
                        Assert.That(newChunk[i], i != changePoint ? Is.EqualTo(chunk[i]) : Is.EqualTo(0));
                    }
                    chunkEvent.Set();
                    break;
                }
            }
        };
        await tcpClient.FetchChunk(0, new IntVector3(0, 0, 0));
        Assert.That(chunkEvent.WaitOne(1000), Is.True);
        for (var i = 0; i < chunk.Length; i++)
        {
            if (chunk[i] == 0) continue;
            _digTarget = new IntVector3(i % 16, i / 256, i / 16 % 16);
        }

        Assert.That(_digTarget, Is.Not.Null);
        await tcpClient.DigChunk(new IntVector3(0, 0, 0), _digTarget ?? new IntVector3(0, 0, 0));
        _digStep++;
        chunkEvent.Reset();
        await tcpClient.FetchChunk(0, new IntVector3(0, 0, 0));
        Assert.Multiple(() =>
        {
            Assert.That(blockChangeEvent.WaitOne(1000), Is.True);
            Assert.That(chunkEvent.WaitOne(1000), Is.True);
        });
        await tcpClient.Disconnect();
    }
}