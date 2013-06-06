using System;

public struct DungeonVector2
{
    public int x;
    public int y;

    public DungeonVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj is DungeonVector2)
            return this == (DungeonVector2)obj;

        return false;
    }

    public override int GetHashCode()
    {
        return (x << 16) | y;
    }

    static public DungeonVector2 Forward
    {
        get { return new DungeonVector2(0, 1); }
    }

    static public DungeonVector2 Backward
    {
        get { return new DungeonVector2(0, -1); }
    }

    static public DungeonVector2 Left
    {
        get { return new DungeonVector2(-1, 0); }
    }

    static public DungeonVector2 Right
    {
        get { return new DungeonVector2(1, 0); }
    }

    static public DungeonVector2 operator+(DungeonVector2 a, DungeonVector2 b)
    {
        return new DungeonVector2(a.x + b.x, a.y + b.y);
    }

    public static DungeonVector2 operator-(DungeonVector2 a, DungeonVector2 b)
    {
        return new DungeonVector2(a.x - b.x, a.y - b.y);
    }

    public static bool operator!=(DungeonVector2 a, DungeonVector2 b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public static bool operator==(DungeonVector2 a, DungeonVector2 b)
    {
        return a.x == b.x && a.y == b.y;
    }
}

