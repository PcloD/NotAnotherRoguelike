using System;

public class DungeonEntityFactory
{
    static public DungeonEntity CreateEntity(DungeonEntityType entityType)
    {
        switch (entityType)
        {
            case DungeonEntityType.Avatar:
                return new DungeonEntityAvatar();

            case DungeonEntityType.Light:
                return new DungeonEntityLight();
        }

        return null;
    }
}


