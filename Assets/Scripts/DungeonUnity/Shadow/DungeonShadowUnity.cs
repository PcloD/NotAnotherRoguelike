using UnityEngine;

public class DungeonShadowUnity : MonoBehaviour
{
    [HideInInspector]
    public Transform trans;

    [HideInInspector]
    public GameObject go;

    public void Awake()
    {
        trans = transform;
        go = gameObject;
    }
}


