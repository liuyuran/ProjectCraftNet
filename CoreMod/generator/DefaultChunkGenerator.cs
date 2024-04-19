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

    private ModManager.game.generator.BlockData[] GenerateMainWorld(int chunkSize, IntVector3 chunkPosition)
    {
        var blockData = new ModManager.game.generator.BlockData[chunkSize * chunkSize * chunkSize];
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
                    blockData[GetBlockIndex(x, y, z, chunkSize)] = new ModManager.game.generator.BlockData
                    {
                        BlockId = blockId
                    };
                }
            }
        }

        return blockData;
    }

    public ModManager.game.generator.BlockData[] GenerateChunkBlockData(int chunkSize, IntVector3 chunkPosition)
    {
        return GenerateMainWorld(chunkSize, chunkPosition);
    }
}