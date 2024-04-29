using CoreMod.blocks;
using ModManager.game.block;
using ModManager.game.generator;
using ModManager.utils;

namespace CoreMod.generator;

/// <summary>
/// 初始世界区块生成器
/// </summary>
public class DefaultChunkGenerator : IChunkGenerator
{
    private readonly FastNoiseLite _noise = new();

    public DefaultChunkGenerator()
    {
        _noise.SetSeed(DateTime.Now.Millisecond);
        _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
    }

    private static int GetBlockIndex(int x, int y, int z, int size)
    {
        if (x > size) x %= size;
        if (y > size) y %= size;
        if (z > size) z %= size;
        return z * size * size + y * size + x;
    }
    
    /// <summary>
    /// 根据区块坐标匹配生物群系
    /// </summary>
    /// <param name="chunkPosition">区块坐标</param>
    /// <returns>生物群系</returns>
    private static EBiome GetBiome(IntVector3 chunkPosition)
    {
        return EBiome.Plain;
    }

    /// <summary>
    /// 生成高度图
    /// </summary>
    /// <param name="chunkData">高度图存储器的引用</param>
    /// <param name="chunkSize">区块大小</param>
    /// <param name="chunkPosition">区块位置</param>
    private static void GenerateHeightMap(
        MemoryChunkData chunkData,
        int chunkSize,
        IntVector3 chunkPosition)
    {
        
    }

    /// <summary>
    /// 计算高度图并据此生成区块数据
    /// </summary>
    /// <param name="chunkSize">区块大小</param>
    /// <param name="chunkPosition">区块位置</param>
    /// <returns>区块数据</returns>
    private MemoryChunkData GenerateMainWorld(int chunkSize, IntVector3 chunkPosition)
    {
        var data = new MemoryChunkData
        {
            Biome = GetBiome(chunkPosition),
            BlockData = new MemoryBlockData[chunkSize * chunkSize * chunkSize]
        };
        GenerateHeightMap(data, chunkSize, chunkPosition);
        for (var x = 0; x < chunkSize; x++)
        {
            for (var z = 0; z < chunkSize; z++)
            {
                var noiseData = _noise.GetNoise(chunkPosition.X * chunkSize + x, chunkPosition.Z * chunkSize + z);
                noiseData = Math.Abs(noiseData);
                var height = (uint)Math.Round(noiseData * chunkSize);
                for (var y = 0; y < chunkSize; y++)
                {
                    var blockId = y > height ? BlockManager.GetBlockId<Air>() : BlockManager.GetBlockId<Dirt>();
                    data.BlockData[GetBlockIndex(x, y, z, chunkSize)] = new MemoryBlockData
                    {
                        BlockId = blockId
                    };
                }
            }
        }

        return data;
    }

    public MemoryChunkData GenerateChunkBlockData(int chunkSize, IntVector3 chunkPosition)
    {
        return GenerateMainWorld(chunkSize, chunkPosition);
    }
}