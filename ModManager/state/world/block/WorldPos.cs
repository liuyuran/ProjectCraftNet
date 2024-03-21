using ModManager.config;
using ModManager.state.world.chunk;

namespace ModManager.state.world.block;

public sealed class WorldPos(long x, long y, long z, long worldId)
{
    public long GetWorldId()
    {
        return worldId;
    }

    public ChunkPos GetChunkPos()
    {
        long chunkSize = ConfigUtil.Instance.GetConfig().Core!.ChunkSize;
        return new ChunkPos((int)(x / chunkSize), (int)(y / chunkSize), (int)(z / chunkSize));
    }

    public BlockPos GetBlockPos()
    {
        long chunkSize = ConfigUtil.Instance.GetConfig().Core!.ChunkSize;
        return new BlockPos((int)(x % chunkSize), (int)(y % chunkSize), (int)(z % chunkSize));
    }
}