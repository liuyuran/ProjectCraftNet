using System.Diagnostics.CodeAnalysis;
using ModManager.game.generator;
using ModManager.utils;

namespace CoreMod.generator;

/// <summary>
/// 初始世界区块生成器
/// </summary>
public class DefaultChunkGenerator : IChunkGenerator
{
    private readonly FastNoiseLite _noisePrimal = new();
    private const int Scale = 16;

    public DefaultChunkGenerator()
    {
        var seed = DateTime.Now.Millisecond;
        _noisePrimal.SetSeed(seed);
        _noisePrimal.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
    }

    private static int GetBlockIndex(int x, int y, int z, int size)
    {
        if (x > size) x %= size;
        if (y > size) y %= size;
        if (z > size) z %= size;
        return z * size * size + y * size + x;
    }
    
    private float GetNoise(int chunkX, int chunkZ, int chunkSize)
    {
        var noiseX = chunkX * chunkSize / Scale;
        var noiseZ = chunkZ * chunkSize / Scale;
        return _noisePrimal.GetNoise(noiseX, noiseZ);
    }

    /// <summary>
    /// 根据区块坐标匹配生物群系
    /// </summary>
    /// <param name="chunkPosition">区块坐标</param>
    /// <param name="chunkSize">区块大小</param>
    /// <param name="level">检索深度</param>
    /// <returns>生物群系</returns>
    private EBiome GetBiome(IntVector3 chunkPosition, int chunkSize, int level = 5)
    {
        /*
         *  这种层在第二阶段也被使用，与第一阶段的混合表示法不同，第二阶段的幻数直接表示生物群系。
            如果自己不是海，四周（象）其中一个是海，那自己就会变成海（除非自己是寒冷，就还是寒冷）。
            否则如果四周（象）也是海，那自己就还是海。
            否则变为炎热，有一定概率变为四周（象）的生物群系。
         */
        if ((chunkPosition / 4) is { X: 0, Z: 0 }) return EBiome.Plain;
        var noiseData = GetNoise(chunkPosition.X, chunkPosition.Z, chunkSize);
        var isLand = noiseData > -.3f;
        if (level <= 0) return isLand ? EBiome.Plain : EBiome.Sea;
        if (isLand)
        {
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    if (Math.Abs(i) == Math.Abs(j)) continue;
                    var noise = GetBiome(chunkPosition + new IntVector3(i, 0, j), chunkSize, level - 1);
                    if (noise == EBiome.Plain)
                    {
                        return EBiome.Plain;
                    }
                }                
            }

            return EBiome.Sea;
        }
        return EBiome.Sea;

        // var flag = 0;
        // for (var i = -1; i <= 1; i++)
        // {
        //     for (var j = -1; j <= 1; j++)
        //     {
        //         if (i == 0 && j == 0) continue;
        //         if (Math.Abs(i) == Math.Abs(j)) continue;
        //         var noise = GetBiome(chunkPosition + new IntVector3(i, 0, j), chunkSize, level - 1);
        //         if (noise == EBiome.Plain)
        //         {
        //             flag++;
        //         }
        //     }                
        // }
        //
        // if (flag == 0) return EBiome.Sea;
        // var percent = Random.Shared.NextSingle() * 4;
        // return percent < flag ? EBiome.Plain : EBiome.Sea;
    }

    /// <summary>
    /// 生成高度图
    /// </summary>
    /// <param name="chunkData">高度图存储器的引用</param>
    /// <param name="chunkSize">区块大小</param>
    /// <param name="chunkPosition">区块位置</param>
    [SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
    private void GenerateHeightMap(
        MemoryChunkData chunkData,
        int chunkSize,
        IntVector3 chunkPosition)
    {
        var lightBlocks = new long[chunkSize, chunkSize];
        for (var x = 0; x < chunkSize; x++)
        {
            for (var z = 0; z < chunkSize; z++)
            {
                lightBlocks[x, z] = chunkData.Biome switch
                {
                    EBiome.Plain => 32,
                    EBiome.Sea => 0,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        chunkData.HeightMap[EHeightMapUsage.LIGHT_BLOCKING] = lightBlocks;
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
            Biome = GetBiome(chunkPosition, chunkSize),
            BlockData = new MemoryBlockData[chunkSize * chunkSize * chunkSize],
        };
        GenerateHeightMap(data, chunkSize, chunkPosition);
        // for (var x = 0; x < chunkSize; x++)
        // {
        //     for (var z = 0; z < chunkSize; z++)
        //     {
        //         var height = (uint)data.HeightMap[EHeightMapUsage.LIGHT_BLOCKING][x,z];
        //         for (var y = 0; y < chunkSize; y++)
        //         {
        //             var blockId = y > height ? BlockManager.GetBlockId<Air>() : BlockManager.GetBlockId<Dirt>();
        //             data.BlockData[GetBlockIndex(x, y, z, chunkSize)] = new MemoryBlockData
        //             {
        //                 BlockId = blockId
        //             };
        //         }
        //     }
        // }

        return data;
    }

    public MemoryChunkData GenerateChunkBlockData(int chunkSize, IntVector3 chunkPosition)
    {
        return GenerateMainWorld(chunkSize, chunkPosition);
    }
}