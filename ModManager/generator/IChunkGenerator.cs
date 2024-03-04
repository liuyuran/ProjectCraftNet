using System.Numerics;

namespace ModManager.generator;

public interface IChunkGenerator
{
    public BlockData[] GenerateChunkBlockData(ulong worldId, Vector3 chunkPosition);
}