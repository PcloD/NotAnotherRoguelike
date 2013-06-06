using System;

public class DungeonEventEntityRotated : DungeonEvent
{
    public DungeonEntity entity;
    public DungeonRotation from;
    public DungeonRotation to;

    public DungeonEventEntityRotated(DungeonEntity entity, DungeonRotation from, DungeonRotation to) : base(DungeonEventType.EntityRotated)
    {
        this.entity = entity;
        this.from = from;
        this.to = to;
    }
}


