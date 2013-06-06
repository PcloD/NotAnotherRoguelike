using System;

public class DungeonEventEntityMoved : DungeonEvent
{
    public DungeonEntity entity;
    public DungeonVector2 from;
    public DungeonVector2 to;

    public DungeonEventEntityMoved(DungeonEntity entity, DungeonVector2 from, DungeonVector2 to) : base(DungeonEventType.EntityMoved)
    {
        this.entity = entity;
        this.from = from;
        this.to = to;
    }
}


