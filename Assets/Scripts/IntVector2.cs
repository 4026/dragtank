using System;

public class IntVector2
{
    /// <summary>
    /// Enum of possible rotations that can be applied to an IntVector2
    /// </summary>
    public enum Rotation { deg0, deg90, deg180, deg270 }

    public int x, y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b) {
        return new IntVector2(a.x + b.x, a.y + b.y);
    }
    
    public static IntVector2 operator -(IntVector2 a, IntVector2 b) {
        return new IntVector2(a.x - b.x, a.y - b.y);
    }
    
    public static IntVector2 operator *(IntVector2 a, int b) {
        return new IntVector2(a.x * b, a.y * b);
    }

    public static IntVector2 operator /(IntVector2 a, int b)
    {
        return new IntVector2(a.x / b, a.y / b);
    }

    /// <summary>
    /// Get a copy of this vector, rotated by the provided rotation.
    /// </summary>
    public IntVector2 Rotated(Rotation by)
    {
        switch (by)
        {
            case Rotation.deg0:
                return new IntVector2(x, y);
            case Rotation.deg90:
                return new IntVector2(-y, x);
            case Rotation.deg180:
                return new IntVector2(-x, -y);
            case Rotation.deg270:
                return new IntVector2(y, -x);
            default:
                throw new ArgumentOutOfRangeException("Unrecognised IntVector2 rotation " + by);
        }
    }
}