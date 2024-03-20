using System.Numerics;

namespace ModManager.archive;

public struct ChunkChanged
{
    public Vector3 Pos;
    public long WorldId;
    public ecs.components.BlockData[] Data;
}