using System.Numerics;
using ModManager.utils;

namespace ModManager.game.generator;

/// <summary>
/// 区块生成器接口
/// </summary>
public interface IChunkGenerator
{
    /// <summary>
    /// 生成区块数据
    /// </summary>
    /// <param name="chunkSize">区块边长，目前锁定为64</param>
    /// <param name="chunkPosition">区块坐标，单位为1</param>
    /// <returns>区块数据</returns>
    public MemoryChunkData GenerateChunkBlockData(int chunkSize, IntVector3 chunkPosition);
}