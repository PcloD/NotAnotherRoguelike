using System;

public abstract class DungeonEntity
{
    private int id;
    private DungeonMap dungeon;
    private DungeonVector2 position;
    private DungeonEntityType type;
    private DungeonRotation rotation;

    public DungeonVector2 Position
    {
        get { return position; }
    }

    public DungeonRotation Rotation
    {
        get { return rotation; }
    }

    public DungeonMap Dungeon
    {
        get { return dungeon; }
    }

    public DungeonEntityType Type
    {
        get { return type; }
    }

    public int Id
    {
        get { return id; }
    }

    public DungeonVector2 Forward
    {
        get { return DungeonVector2.FromRotation(rotation); }
    }

    public DungeonVector2 Back
    {
        get { return -DungeonVector2.FromRotation(rotation); }
    }

    public DungeonEntity(DungeonEntityType type)
    {
        this.type = type;
    }

    internal void OnAddedToDungeon(DungeonMap dungeon, DungeonVector2 position, DungeonRotation rotation, int id)
    {
        this.dungeon = dungeon;
        this.position = position;
        this.rotation = rotation;
        this.id = id;
    }

    internal void OnRemovedFromDungeon()
    {
        dungeon = null;
        id = -1;
    }

    public bool CanMove(DungeonVector2 delta)
    {
        return CanMoveTo(position + delta);
    }

    public bool CanMoveTo(DungeonVector2 position)
    {
        if (!dungeon.CheckValidPosition(position.x, position.y))
            return false;

        return true;
    }

    public void Move(DungeonVector2 delta)
    {
        MoveTo(position + delta);
    }

    public void MoveTo(DungeonVector2 newPosition)
    {
        if (newPosition != position)
        {
            if (!dungeon.CheckValidPosition(newPosition.x, newPosition.y))
                throw new ArgumentException("Invalid entity position");

            DungeonVector2 oldPosition = position;

            this.position = newPosition;

            dungeon.ReportDungeonEvent(DungeonEventFactory.CreateEntityMoved(this, oldPosition, newPosition));
        }
    }

    public void Rotate(DungeonRotation newRotation)
    {
        if (newRotation != rotation)
        {
            DungeonRotation oldRotation = this.rotation;

            this.rotation = newRotation;

            dungeon.ReportDungeonEvent(DungeonEventFactory.CreateEntityRotated(this, oldRotation, newRotation));
        }
    }

    public bool IsVisible()
    {
        return dungeon.GetTile(Position.x, Position.y).visible;
    }
}


