using System.Numerics;
using ModManager.game.block;
using ModManager.utils;

namespace ModManager.game.generator;

/// <summary>
/// 初始世界区块生成器
/// </summary>
public class DefaultChunkGenerator: IChunkGenerator
{
    private readonly FastNoiseLite _noise = new();
    private static int GetBlockIndex(int x, int y, int z, int size)
    {
        if (x > size) x %= size;
        if (y > size) y %= size;
        if (z > size) z %= size;
        return z * size * size + y * size + x;
    }

    private void GenerateMainWorld(ref BlockData[] blockData, int chunkSize, Vector3 chunkPosition)
    {
        for (var x = 0; x < chunkSize; x++)
        {
            for (var z = 0; z < chunkSize; z++)
            {
                var noiseData = _noise.GetNoise(chunkPosition.X * chunkSize + x, chunkPosition.Z * chunkSize + z);
                noiseData = Math.Abs(noiseData);
                var height = (uint) Math.Round(noiseData * chunkSize);
                for (var y = 0; y < chunkSize; y++)
                {
                    var block = y > height ? BlockEnum.Air : BlockEnum.Dirt;
                    var blockId = BlockManager.GetBlockId(block);
                    blockData[GetBlockIndex(x, y, z, chunkSize)] = new BlockData
                    {
                        BlockId = blockId
                    };
                }
            }
        }        
    }
    
    public BlockData[] GenerateChunkBlockData(int chunkSize, Vector3 chunkPosition)
    {
        _noise.SetSeed(DateTime.Now.Millisecond);
        _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        var blockData = new BlockData[chunkSize * chunkSize * chunkSize];
        GenerateMainWorld(ref blockData, chunkSize, chunkPosition);
        return blockData;
    }
}