using UnityEngine;

public class DungeonEventHandlerEntityMoved : DungeonEventHandlerUnity
{
    private const float MOVE_TIME = 0.25f;

    private DungeonEntityUnity entity;
    private Vector3 moveFrom;
    private Vector3 moveTo;
    private float time;

    public DungeonEventHandlerEntityMoved(DungeonUnity dungeon, DungeonEventEntityMoved dungeonEvent)
    {
        entity = dungeon.GetEntityUnity(dungeonEvent.entity);
        moveFrom = dungeon.GetWorldPosition(dungeonEvent.from);
        moveTo = dungeon.GetWorldPosition(dungeonEvent.to);
    }

    public override bool Update()
    {
        time += Time.deltaTime;

        if (time < MOVE_TIME)
        {
            entity.trans.position = Vector3.Lerp(moveFrom, moveTo, time / MOVE_TIME);
            return false;
        }
        else
        {
            entity.trans.position = moveTo;
            return true;
        }
    }
}


