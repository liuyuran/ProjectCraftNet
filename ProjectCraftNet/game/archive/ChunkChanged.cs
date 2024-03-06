using System.Numerics;

namespace ProjectCraftNet.game.archive;

public struct ChunkChanged
{
    public Vector3 Pos;
    public long WorldId;
    public components.BlockData[] Data;
}