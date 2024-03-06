namespace ProjectCraftNet.game.components;

public struct BlockData
{
    public long BlockId;
}

public struct ChunkBlockData
{
    public long WorldId;
    public BlockData[] Data;
}