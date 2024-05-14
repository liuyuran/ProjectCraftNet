namespace ModManager.game.generator;

/// <summary>
/// 在内存中存储的区块数据结构
/// </summary>
public class MemoryChunkData
{
    public EBiome Biome;
    // 高度图合集
    public readonly Dictionary<EHeightMapUsage, long[,]> HeightMap = new();
    public required MemoryBlockData[] BlockData;
}