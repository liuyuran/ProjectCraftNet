using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Arch.Core.Utils;

namespace ProjectCraftNet.game.components;

public struct Position
{
    public Vector3 Val;
}

public struct Rotation
{
    public Vector3 Val;
}

public struct Sight
{
    public Vector3 Val;
}

public struct Player {}

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible")]
public class Archetypes
{
    public static ComponentType[] Player = [typeof(Player), typeof(Position), typeof(Rotation), typeof(Sight)];
}