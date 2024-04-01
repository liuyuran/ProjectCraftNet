using System.Numerics;
using ModManager.utils;

namespace ModManager.archive;

public struct ChunkChanged
{
    public IntVector3 Pos;
    public long WorldId;
    public long[] Data;
}