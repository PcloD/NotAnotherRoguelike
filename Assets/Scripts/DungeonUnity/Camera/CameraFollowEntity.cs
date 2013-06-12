using UnityEngine;

public class CameraFollowEntity : MonoBehaviour
{
    public Vector3 moveDelta;
    public Vector3 lookAtDelta;

    public DungeonEntityUnity entity;

    private Vector3 lastPosition;
    private Vector3 lastLookAtPosition;

    private Transform trans;

    private bool animating;
    private Vector3 newPosition;
    private Vector3 newLookAtPosition;
    private float animationTime;
    private float animationDuration;
    private DungeonEntityUnity newEntity;

    public void Awake()
    {
        trans = transform;
    }

    public void Update()
    {
        if (entity)
        {
            lastPosition = entity.trans.position + moveDelta;
            lastLookAtPosition = entity.trans.position + lookAtDelta;

            trans.position = lastPosition;
            trans.LookAt(lastLookAtPosition);
        }
        else if (animating)
        {
            float t = animationTime / animationDuration;

            trans.position = Vector3.Lerp(lastPosition, newPosition, t);
            trans.LookAt(Vector3.Lerp(lastLookAtPosition, newLookAtPosition, t));

            if (t >= 1.0f)
            {
                animating = false;
                lastPosition = newPosition;
                lastLookAtPosition = newLookAtPosition;
                entity = newEntity;
            }

            animationTime += Time.deltaTime;
        }
    }

    public void AnimateTo(Vector3 newPosition, Vector3 newLookAtPosition, float duration)
    {
        animating = true;

        this.newLookAtPosition = newLookAtPosition;
        this.newPosition = newPosition;
        this.animationDuration = duration;
        this.animationTime = 0.0f;
        newEntity = null;
    }

    
    public void AnimateTo(DungeonEntityUnity entity, float duration)
    {
        animating = true;

        this.newPosition = entity.trans.position + moveDelta;
        this.newLookAtPosition = entity.trans.position + lookAtDelta;

        this.newEntity = entity;
        this.animationDuration = duration;
        this.animationTime = 0.0f;
    }}


