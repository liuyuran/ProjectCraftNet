namespace ModManager.game.generator;

/// <summary>
/// 在内存中存储的区块数据结构
/// </summary>
public class MemoryChunkData
{
    public EBiome Biome;
    public Dictionary<EHeightMapUsage, long[,]> HeightMap = new();
    public required MemoryBlockData[] BlockData;
}