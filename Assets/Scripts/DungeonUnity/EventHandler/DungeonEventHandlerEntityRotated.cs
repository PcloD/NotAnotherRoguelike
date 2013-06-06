using UnityEngine;

public class DungeonEventHandlerEntityRotated : DungeonEventHandlerUnity
{
    private const float ROTATE_TIME = 0.1f;

    private DungeonEntityUnity entity;
    private Quaternion rotateFrom;
    private Quaternion rotateTo;
    private float time;

    public DungeonEventHandlerEntityRotated(DungeonUnity dungeon, DungeonEventEntityRotated dungeonEvent)
    {
        entity = dungeon.GetEntityUnity(dungeonEvent.entity);
        rotateFrom = dungeon.GetWorldRotation(dungeonEvent.from);
        rotateTo = dungeon.GetWorldRotation(dungeonEvent.to);
    }

    public override bool Update()
    {
        time += Time.deltaTime;

        if (time < ROTATE_TIME)
        {
            entity.trans.rotation = Quaternion.Lerp(rotateFrom, rotateTo, time / ROTATE_TIME);
            return false;
        }
        else
        {
            entity.trans.rotation = rotateTo;
            return true;
        }
    }
}


