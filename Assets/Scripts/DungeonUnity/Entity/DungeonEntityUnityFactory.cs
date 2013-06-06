using UnityEngine;

public class DungeonEntityUnityFactory : MonoBehaviour
{
    public GameObject avatarPrefab;

    public DungeonEntityUnity CreateEntity(DungeonEntity entity)
    {
        switch (entity.Type)
        {
            case DungeonEntityType.Avatar:
                return ((GameObject)GameObject.Instantiate(avatarPrefab)).GetComponent<DungeonEntityUnity>();
        }

        return null;
    }

    public void DestroyEntity(DungeonEntityUnity entityUnity)
    {
        GameObject.Destroy(entityUnity.gameObject);
    }
}


