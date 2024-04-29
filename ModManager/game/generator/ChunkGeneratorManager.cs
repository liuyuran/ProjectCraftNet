using Microsoft.Extensions.Logging;
using ModManager.config;
using ModManager.exception;
using ModManager.logger;
using ModManager.state;
using ModManager.utils;
using static ModManager.game.localization.LocalizationManager;
using static ModManager.mod.ModManager;

namespace ModManager.game.generator;

/// <summary>
/// 区块生成器，用于进行世界生成
/// </summary>
public class ChunkGeneratorManager
{
    private readonly struct QueueItem(long worldId, IntVector3 pos): IEquatable<QueueItem>
    {
        public readonly IntVector3 Pos = pos;
        public readonly long WorldId = worldId;
        public bool Equals(QueueItem other)
        {
            return Pos == other.Pos && WorldId == other.WorldId; 
        }
    }
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(ChunkGeneratorManager));
    private static ChunkGeneratorManager Instance { get; } = new();
    private readonly Dictionary<long, IChunkGenerator> _generators = new();
    private readonly Queue<QueueItem> _generatingChunks = new();
    private readonly Dictionary<QueueItem, MemoryBlockData[]> _generatedChunks = new();

    private ChunkGeneratorManager()
    {
        Task.Run(() =>
        {
            var chunkSize = ConfigUtil.Instance.GetConfig().Core!.ChunkSize;
            while (!CraftNet.Instance.CancelToken.IsCancellationRequested)
            {
                while (_generatingChunks.Count > 0)
                {
                    var centerPosition = _generatingChunks.Dequeue();
                    var generator = _generators[centerPosition.WorldId];
                    Logger.LogDebug("Generate chunk at [{x}, {y}, {z}]", centerPosition.Pos.X, centerPosition.Pos.Y, centerPosition.Pos.Z);
                    var result = generator.GenerateChunkBlockData(chunkSize, centerPosition.Pos);
                    _generatedChunks[centerPosition] = result.BlockData;
                }
                Thread.Sleep(1000);
            }
        }, CraftNet.Instance.CancelToken.Token);
    }
    
    /// <summary>
    /// 注册区块生成器
    /// </summary>
    /// <param name="worldId">世界编码</param>
    /// <param name="generator">生成器实例</param>
    public static void AddChunkGenerator(long worldId, IChunkGenerator generator)
    {
        if (Instance._generators.TryAdd(worldId, generator)) return;
        Logger.LogWarning("{}", Localize(ModId, "Chunk generator for world {0} already exists, override it", worldId));
        Instance._generators[worldId] = generator;
    }
    
    /// <summary>
    /// 删除区块生成器
    /// </summary>
    /// <param name="worldId">世界编码</param>
    public static void RemoveChunkGenerator(long worldId)
    {
        if (Instance._generators.Remove(worldId)) return;
        Logger.LogWarning("{}", Localize(ModId, "Chunk generator for world {0} not found", worldId));
    }
    
    /// <summary>
    /// 异步生成特定世界特定区块的基准初始数据
    /// </summary>
    /// <param name="worldId">世界编码</param>
    /// <param name="chunkPosition">区块坐标，以1为单位</param>
    /// <returns>区块数据，如果正在生成中则返回null</returns>
    public static MemoryBlockData[]? GenerateChunkBlockData(long worldId, IntVector3 chunkPosition)
    {
        if (Instance._generatedChunks.TryGetValue(new QueueItem(worldId, chunkPosition), out var result))
        {
            Instance._generatedChunks.Remove(new QueueItem(worldId, chunkPosition));
            return result;
        }
        if (Instance._generators.ContainsKey(worldId))
        {
            Instance._generatingChunks.Enqueue(new QueueItem(worldId, chunkPosition));
            return null;
        }
        Logger.LogWarning("No chunk generator found for world {}", worldId);
        throw new NoSuchChunkGenerator(worldId);
    }
}