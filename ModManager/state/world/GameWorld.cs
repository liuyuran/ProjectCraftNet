﻿using ModManager.config;
using ModManager.state.world.block;
using ModManager.state.world.block.interfaces;
using ModManager.state.world.chunk;

namespace ModManager.state.world;

public class GameWorld
{
    private readonly Dictionary<long, Dictionary<ChunkPos, long[]>> _chunkLink = new();
    
    public IBlockState? GetBlockState(WorldPos pos)
    {
        if (!_chunkLink.TryGetValue(pos.GetWorldId(), out var chunkLink)) return null;
        var chunkPos = pos.GetChunkPos();
        if (!chunkLink.TryGetValue(chunkPos, out var chunkData)) return null;
        var blockPos = pos.GetBlockPos();
        var blockId = chunkData[GetIndexFromBlockPos(blockPos)];
        var state = new BlockState(blockId, blockPos);
        return state;
    }
    
    private static int GetIndexFromBlockPos(BlockPos pos)
    {
        var len = ConfigUtil.Instance.GetConfig().Core!.ChunkSize;
        return pos.Z * len * len + pos.Y * len + pos.X;
    }
    
    public void SetBlockToChunk(long worldId, ChunkPos chunkPos, BlockPos blockPos, long blockId)
    {
        if (!_chunkLink.TryGetValue(worldId, out var chunkLink)) return;
        if (!chunkLink.TryGetValue(chunkPos, out var chunkData)) return;
        chunkData[GetIndexFromBlockPos(blockPos)] = blockId;
    }
    
    public void AddChunk(long worldId, ChunkPos pos, long[] data)
    {
        if (!_chunkLink.TryGetValue(worldId, out var chunkLink))
        {
            chunkLink = new Dictionary<ChunkPos, long[]>();
            _chunkLink[worldId] = chunkLink;
        }
        chunkLink[pos] = data;
    }
}