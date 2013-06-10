using UnityEngine;

public class DungeonEntityUnity : MonoBehaviour
{
    public DungeonEntityType entityType;

    [HideInInspector]
    public Transform trans;

    [HideInInspector]
    public DungeonUnity dungeonUnity;

    public DungeonEntity entity;

    public virtual void Awake()
    {
        trans = transform;
    }

    public void Init(DungeonUnity dungeonUnity, DungeonEntity entity)
    {
        this.dungeonUnity = dungeonUnity;
        this.entity = entity;

        trans.position = dungeonUnity.GetWorldPosition(entity.Position);
        trans.rotation = dungeonUnity.GetWorldRotation(entity.Rotation);
    }
}


