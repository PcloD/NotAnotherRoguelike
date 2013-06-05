using System;

public class DungeonEntity
{
    private Dungeon dungeon;
    private DungeonVector2 position;
    private DungeonEntityType type;

    public DungeonVector2 Position
    {
        get { return position; }
    }

    public Dungeon Dungeon
    {
        get { return dungeon; }
    }

    public DungeonEntityType Type
    {
        get { return type; }
    }

    public DungeonEntity(DungeonEntityType type)
    {
        this.type = type;
    }

    internal void OnAddedToDungeon(Dungeon dungeon, DungeonVector2 position)
    {
        this.dungeon = dungeon;
        this.position = position;
    }

    internal void OnRemovedFromDungeon()
    {
        dungeon = null;
    }

    public void Move(DungeonVector2 delta)
    {
        if (dungeon != null)
        {
            if (!dungeon.CheckValidPosition(position.x + delta.x, position.y + delta.y))
                throw new ArgumentException("Invalid entity position");
        }

        position.x += delta.x;
        position.y += delta.y;
    }

    public void MoveTo(int x, int y)
    {
        if (dungeon != null)
        {
            if (!dungeon.CheckValidPosition(x, y))
                throw new ArgumentException("Invalid entity position");
        }

        position.x = x;
        position.y = y;
    }
}


