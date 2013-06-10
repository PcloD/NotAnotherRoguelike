using UnityEngine;

public class DungeonTileSideUnity : MonoBehaviour
{
    public string id;

    [HideInInspector]
    public GameObject go;

    [HideInInspector]
    public Transform trans;

    public void Awake()
    {
        go = gameObject;
        trans = transform;
    }
}

