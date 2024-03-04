using System.Numerics;
using Microsoft.Extensions.Logging;
using ModManager.logger;

namespace ModManager.generator;

/// <summary>
/// 区块生成器，用于进行世界生成
/// </summary>
public class ChunkGeneratorManager
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(ChunkGeneratorManager));
    private static ChunkGeneratorManager Instance { get; } = new();
    private readonly Dictionary<ulong, IChunkGenerator> _generators = new();

    private ChunkGeneratorManager()
    {
        _generators.Add(0, new DefaultChunkGenerator());
    }
    
    public static BlockData[] GenerateChunkBlockData(ulong worldId, Vector3 chunkPosition)
    {
        if (Instance._generators.TryGetValue(worldId, out var generator))
            return generator.GenerateChunkBlockData(worldId, chunkPosition);
        Logger.LogWarning("No chunk generator found for world {}", worldId);
        return Array.Empty<BlockData>();
    }
}