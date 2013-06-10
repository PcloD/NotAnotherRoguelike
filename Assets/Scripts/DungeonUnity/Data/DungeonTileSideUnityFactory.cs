using UnityEngine;
using System.Collections.Generic;

public class DungeonTileSideUnityFactory : MonoBehaviour
{
    public DungeonTileSideUnity[] sidesPrefabs;

    private Transform poolContainer;

    private Dictionary<string, GameObject> sidesPrefabsDictionary;

    private Dictionary<string, List<DungeonTileSideUnity>> pool = new Dictionary<string, List<DungeonTileSideUnity>>();

    public void Awake()
    {
        poolContainer = new GameObject("SidePool").transform;
        poolContainer.parent = transform;
        poolContainer.localPosition = Vector3.zero;
        poolContainer.localRotation = Quaternion.identity;

        sidesPrefabsDictionary = new Dictionary<string, GameObject>();
        for (int i = 0; i < sidesPrefabs.Length; i++)
            sidesPrefabsDictionary.Add(sidesPrefabs[i].id, sidesPrefabs[i].gameObject);
    }

    public GameObject GetPrefab(string id)
    {
        return sidesPrefabsDictionary[id];
    }

    public DungeonTileSideUnity GetSide(string id)
    {
        DungeonTileSideUnity side = null;

        if (pool.ContainsKey(id) && pool[id].Count > 0)
        {
            List<DungeonTileSideUnity> sidePool = pool[id];

            side = sidePool[sidePool.Count - 1];
            sidePool.RemoveAt(sidePool.Count - 1);

            side.go.SetActive(true);
        }
        else
        {
            GameObject prefab = GetPrefab(id);

            side = ((GameObject)GameObject.Instantiate(prefab)).GetComponent<DungeonTileSideUnity>();
        }

        return side;
    }

    public void ReturnSide(DungeonTileSideUnity side)
    {
        List<DungeonTileSideUnity> sidePool;

        if (pool.ContainsKey(side.id))
        {
            sidePool = pool[side.id];
        }
        else
        {
            sidePool = new List<DungeonTileSideUnity>();
            pool.Add(side.id, sidePool);
        }

        side.trans.parent = poolContainer;
        side.go.SetActive(false);
        sidePool.Add(side);
    }
}
