namespace ModManager.utils;

public class IntVector3(int x, int y, int z)
{
    public readonly int X = x;
    public readonly int Y = y;
    public readonly int Z = z;
    
    public static IntVector3 operator +(IntVector3 a, IntVector3 b)
    {
        return new IntVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
    
    public static IntVector3 operator -(IntVector3 a, IntVector3 b)
    {
        return new IntVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
    
    public static IntVector3 operator *(IntVector3 a, int b)
    {
        return new IntVector3(a.X * b, a.Y * b, a.Z * b);
    }
    
    public static IntVector3 operator /(IntVector3 a, int b)
    {
        return new IntVector3(a.X / b, a.Y / b, a.Z / b);
    }
    
    public static bool operator ==(IntVector3 a, IntVector3 b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }
    
    public static bool operator !=(IntVector3 a, IntVector3 b)
    {
        return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is IntVector3 other && X == other.X && Y == other.Y && Z == other.Z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }
}