using UnityEngine;

public class CameraFollowEntity : MonoBehaviour
{
    public Vector3 moveDelta;
    public Vector3 lookAtDelta;
    public Vector3 firstPersonDelta = new Vector3(0.0f, 0.5f, 0.0f);

    public DungeonEntityUnity entity;
    public bool firstPerson;

    private Vector3 lastPosition;
    private Vector3 lastLookAtPosition;

    private Transform trans;

    private bool animating;
    private Vector3 newCameraPosition;
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
            if (firstPerson)
            {
                trans.position = entity.trans.position + firstPersonDelta;
                trans.LookAt(trans.position + entity.trans.forward);
            }
            else
            {
                lastPosition = entity.trans.position + moveDelta;
                lastLookAtPosition = entity.trans.position + lookAtDelta;

                trans.position = lastPosition;
                trans.LookAt(lastLookAtPosition);
            }
        }
        else if (animating)
        {
            float t = animationTime / animationDuration;

            trans.position = Vector3.Lerp(lastPosition, newCameraPosition, t);
            trans.LookAt(Vector3.Lerp(lastLookAtPosition, newLookAtPosition, t));

            if (t >= 1.0f)
            {
                animating = false;
                lastPosition = newCameraPosition;
                lastLookAtPosition = newLookAtPosition;
                entity = newEntity;
            }

            animationTime += Time.deltaTime;
        }
    }

    public void AnimateTo(Vector3 newCameraPosition, Vector3 newLookAtPosition, float duration)
    {
        animating = true;

        this.newCameraPosition = newCameraPosition;
        this.newLookAtPosition = newLookAtPosition;

        this.animationDuration = duration;
        this.animationTime = 0.0f;
        newEntity = null;
    }

    public void AnimateTo(Vector3 newPosition, float duration)
    {
        AnimateTo(newPosition + moveDelta, newPosition + lookAtDelta, duration);
    }

    public void AnimateTo(DungeonEntityUnity entity, float duration)
    {
        animating = true;

        this.newCameraPosition = entity.trans.position + moveDelta;
        this.newLookAtPosition = entity.trans.position + lookAtDelta;

        this.newEntity = entity;
        this.animationDuration = duration;
        this.animationTime = 0.0f;
    }

    public bool IsBusy()
    {
        return animating;
    }
}


