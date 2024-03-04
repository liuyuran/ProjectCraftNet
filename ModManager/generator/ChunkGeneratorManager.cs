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
    private ChunkGeneratorManager() {}
    
    public static BlockData[] GenerateChunkBlockData(ulong worldId, Vector3 chunkPosition)
    {
        Logger.LogInformation("GenerateChunkBlockData");
        return new BlockData[0];
    }
}