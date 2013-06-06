using UnityEngine;

public class DungeonEntityUnity : MonoBehaviour
{
    public Transform trans;
    public DungeonUnity dungeonUnity;
    public DungeonEntity entity;

    public void Awake()
    {
        trans = transform;
    }

    public void Init(DungeonUnity dungeonUnity, DungeonEntity entity)
    {
        this.dungeonUnity = dungeonUnity;
        this.entity = entity;

        trans.position = dungeonUnity.GetWorldPosition(entity.Position);
    }
}


