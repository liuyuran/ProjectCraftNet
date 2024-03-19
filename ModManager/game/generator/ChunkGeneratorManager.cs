using System.Numerics;
using Microsoft.Extensions.Logging;
using ModManager.logger;
using static ModManager.localization.LocalizationManager;
using static ModManager.ModManager;

namespace ModManager.generator;

/// <summary>
/// 区块生成器，用于进行世界生成
/// </summary>
public class ChunkGeneratorManager
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(ChunkGeneratorManager));
    private static ChunkGeneratorManager Instance { get; } = new();
    private readonly Dictionary<long, IChunkGenerator> _generators = new();

    private ChunkGeneratorManager()
    {
        // 仅注册主世界的区块生成器
        _generators.Add(0, new DefaultChunkGenerator());
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
    /// 生成特定世界特定区块的基准初始数据
    /// </summary>
    /// <param name="worldId">世界编码</param>
    /// <param name="chunkPosition">区块坐标，以1为单位</param>
    /// <returns>区块数据</returns>
    public static BlockData[] GenerateChunkBlockData(long worldId, Vector3 chunkPosition)
    {
        if (Instance._generators.TryGetValue(worldId, out var generator))
            return generator.GenerateChunkBlockData( 64, chunkPosition);
        Logger.LogWarning("No chunk generator found for world {}", worldId);
        return Array.Empty<BlockData>();
    }
}