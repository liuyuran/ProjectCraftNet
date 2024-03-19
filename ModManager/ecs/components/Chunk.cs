namespace ModManager.ecs.components;

public struct BlockData
{
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public long BlockId { get; set; }
}

public struct ChunkBlockData
{
    public long WorldId;
    public BlockData[] Data;
    public bool Changed;
}