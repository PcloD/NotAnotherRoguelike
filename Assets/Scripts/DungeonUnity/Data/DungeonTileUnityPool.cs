using UnityEngine;
using System.Collections.Generic;

public class DungeonTileUnityPool : MonoBehaviour
{
    private Transform trans;

    private List<DungeonTileUnity> pool = new List<DungeonTileUnity>(1024);

    public void Awake()
    {
        trans = transform;
    }

    public DungeonTileUnity GetTile()
    {
        DungeonTileUnity tile;

        if (pool.Count == 0)
        {
            GameObject go = new GameObject("Tile");
            tile = go.AddComponent<DungeonTileUnity>();
        }
        else
        {
            tile = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            tile.go.SetActive(true);
        }

        return tile;
    }

    public void ReturnTile(DungeonTileUnity tile)
    {
        tile.Clear();
        tile.go.SetActive(false);
        tile.trans.parent = trans;

        pool.Add(tile);
    }
}
