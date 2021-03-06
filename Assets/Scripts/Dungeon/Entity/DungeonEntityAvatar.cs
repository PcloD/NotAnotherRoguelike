using System;

public class DungeonEntityAvatar : DungeonEntity
{
    public DungeonEntityAvatar() : base(DungeonEntityType.Avatar)
    {
    }

    
    public bool CanWalk(DungeonVector2 delta)
    {
        return CanWalkTo(Position + delta);
    }

    public bool CanWalkTo(DungeonVector2 position)
    {
        if (!Dungeon.CheckValidPosition(position.x, position.y))
            return false;

        if (!Dungeon.GetTile(position.x, position.y).walkable)
            return false;

        return true;
    }

    public bool Walk(DungeonVector2 delta, bool rotate = true)
    {
        if (!CanWalk(delta))
            return false;

        if (rotate)
            Rotate(delta.GetRotation());

        MoveTo(Position + delta);

        return true;
    }
}


