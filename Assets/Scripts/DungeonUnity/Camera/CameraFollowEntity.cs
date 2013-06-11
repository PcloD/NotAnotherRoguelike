using UnityEngine;

public class CameraFollowEntity : MonoBehaviour
{
    public Vector3 moveDelta;
    public Vector3 lookAtDelta;

    public DungeonEntityUnity entity;

    private Transform trans;

    public void Awake()
    {
        trans = transform;
    }

    public void Update()
    {
        if (entity)
        {
            trans.position = entity.trans.position + moveDelta;
            trans.LookAt(entity.trans.position + lookAtDelta);
        }
    }
}


