using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Arch.Core.Utils;
using ModManager.game.client;
using ModManager.utils;

namespace ModManager.ecs.components;

public struct Position
{
    public IntVector3 ChunkPos;
    public Vector3 InChunkPos;
}

public struct Rotation
{
    public Vector3 Val;
}

public struct Sight
{
    public Vector3 Val;
}

public struct Player
{
    public long UserId;
    public long SocketId;
    public long WorldId;
    public GameMode GameMode;
}

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class Archetypes
{
    public static readonly ComponentType[] Player = [typeof(Player), typeof(Position), typeof(Rotation), typeof(Sight)];
    public static readonly ComponentType[] Chunk = [typeof(ChunkBlockData), typeof(Position)];
}
