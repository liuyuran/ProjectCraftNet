using System.Numerics;

namespace ModManager.generator;

public class DefaultChunkGenerator: IChunkGenerator
{
    public BlockData[] GenerateChunkBlockData(ulong worldId, Vector3 chunkPosition)
    {
        throw new NotImplementedException();
    }
}