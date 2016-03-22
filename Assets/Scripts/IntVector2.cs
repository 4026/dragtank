using System;

public class IntVector2
{
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
}