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

    static public DungeonVector2 Forward
    {
        get { return new DungeonVector2(0, -1); }
    }

    static public DungeonVector2 Backward
    {
        get { return new DungeonVector2(0, 1); }
    }

    static public DungeonVector2 Left
    {
        get { return new DungeonVector2(-1, 0); }
    }

    static public DungeonVector2 Right
    {
        get { return new DungeonVector2(1, 0); }
    }
}


