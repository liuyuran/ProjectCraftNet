namespace ModManager.utils;

public struct LongVector3(long x, long y, long z)
{

    public readonly long X = x;
    public readonly long Y = y;
    public readonly long Z = z;

    public static LongVector3 operator +(LongVector3 a, LongVector3 b)
    {
        return new LongVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
    
    public static LongVector3 operator -(LongVector3 a, LongVector3 b)
    {
        return new LongVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
    
    public static LongVector3 operator *(LongVector3 a, long b)
    {
        return new LongVector3(a.X * b, a.Y * b, a.Z * b);
    }
    
    public static LongVector3 operator /(LongVector3 a, long b)
    {
        return new LongVector3(a.X / b, a.Y / b, a.Z / b);
    }
    
    public static bool operator ==(LongVector3 a, LongVector3 b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }
    
    public static bool operator !=(LongVector3 a, LongVector3 b)
    {
        return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
    }

    public override bool Equals(object? obj)
    {
        return obj is LongVector3 other && X == other.X && Y == other.Y && Z == other.Z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }
}