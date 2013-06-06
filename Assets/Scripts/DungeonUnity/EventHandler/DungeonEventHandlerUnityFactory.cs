using UnityEngine;

public class DungeonEventHandlerUnityFactory : MonoBehaviour
{
    public DungeonEventHandlerUnity CreateEventHandler(DungeonUnity dungeon, DungeonEvent dungeonEvent)
    {
        switch (dungeonEvent.type)
        {
            case DungeonEventType.EntityMoved:
                return new DungeonEventHandlerEntityMoved(dungeon, (DungeonEventEntityMoved) dungeonEvent);

            case DungeonEventType.EntityRotated:
                return new DungeonEventHandlerEntityRotated(dungeon, (DungeonEventEntityRotated) dungeonEvent);
        }

        return null;
    }
}


