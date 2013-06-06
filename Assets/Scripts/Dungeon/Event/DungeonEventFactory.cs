using System;

public class DungeonEventFactory
{
    static public DungeonEvent CreateEntityMoved(DungeonEntity entity, DungeonVector2 from, DungeonVector2 to)
    {
        return new DungeonEventEntityMoved(entity, from, to);
    }

    static public DungeonEvent CreateEntityRotated(DungeonEntity entity, DungeonRotation from, DungeonRotation to)
    {
        return new DungeonEventEntityRotated(entity, from, to);
    }
}


