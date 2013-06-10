using UnityEngine;
using System.Collections.Generic;

public class DungeonEntityUnityFactory : MonoBehaviour
{
    public DungeonEntityUnity[] entitiesPrefabs;

    private Dictionary<DungeonEntityType, GameObject> entitiesDictionary = new Dictionary<DungeonEntityType, GameObject>();

    public void Awake()
    {
        for (int i = 0; i < entitiesPrefabs.Length; i++)
            entitiesDictionary.Add(entitiesPrefabs[i].entityType, entitiesPrefabs[i].gameObject);
    }

    public GameObject GetPrefab(DungeonEntityType entityType)
    {
        return entitiesDictionary[entityType];
    }

    public DungeonEntityUnity CreateEntity(DungeonEntity entity)
    {
        GameObject prefab = GetPrefab(entity.Type);

        return ((GameObject)GameObject.Instantiate(prefab)).GetComponent<DungeonEntityUnity>();
    }

    public void DestroyEntity(DungeonEntityUnity entityUnity)
    {
        GameObject.Destroy(entityUnity.gameObject);
    }
}


