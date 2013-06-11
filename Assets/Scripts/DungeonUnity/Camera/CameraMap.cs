using UnityEngine;

public class CameraMap : MonoBehaviour
{
    public int minOrthographicSize = 3;
    public int maxOrthographicSize = 30;

    public Vector3 moveDelta;

    public DungeonEntityUnity entity;

    private Transform trans;
    private int orthographicSizeStep;

    public void Awake()
    {
        trans = transform;

        camera.orthographicSize = (maxOrthographicSize + minOrthographicSize) / 2;

        orthographicSizeStep = (maxOrthographicSize + minOrthographicSize) / 8;
    }

    public void Update()
    {
        if (entity)
        {
            trans.position = entity.trans.position + moveDelta;
            trans.LookAt(entity.trans.position);
        }
    }

    public void OnGUI()
    {
        int size = Mathf.Max(Screen.width, Screen.height) / 10;

        if (GUI.Button(new Rect(Screen.width - size - 10, Screen.height / 2 - size - 10, size, size), "+"))
        {
            camera.orthographicSize -= orthographicSizeStep;
            if (camera.orthographicSize < minOrthographicSize)
                camera.orthographicSize = minOrthographicSize;
        }

        if (GUI.Button(new Rect(Screen.width - size - 10, Screen.height / 2, size, size), "-"))
        {
            camera.orthographicSize += orthographicSizeStep;
            if (camera.orthographicSize > maxOrthographicSize)
                camera.orthographicSize = maxOrthographicSize;
        }
    }
}


