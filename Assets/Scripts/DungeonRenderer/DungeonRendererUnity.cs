using UnityEngine;
using System;
using System.Collections.Generic;

public class DungeonRendererUnity : MonoBehaviour, IDungeonRenderer
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    private List<GameObject> floorTiles = new List<GameObject>();
    private List<GameObject> wallTiles = new List<GameObject>();

    private List<GameObject> floorTilesPool = new List<GameObject>();
    private List<GameObject> wallTilesPool = new List<GameObject>();

    public void DrawDungeon(Dungeon dungeon)
    {
        ClearDungeon();

        Transform trans = transform;

        trans.position = Vector3.zero;
        trans.rotation = Quaternion.identity;

        for (int x = 0; x < dungeon.SizeX; x++)
        {
            for (int y = 0; y < dungeon.SizeY; y++)
            {
                DungeonTile tile = dungeon.GetTile(x, y);

                Vector3 floorPosition = new Vector3(x, -1, y);
                Vector3 wallPosition = new Vector3(x, 0, y);

                GameObject floor;

                if (floorTilesPool.Count == 0)
                {
                    floor = (GameObject)GameObject.Instantiate(floorPrefab, floorPosition, Quaternion.identity);
                    floor.transform.parent = trans;
                }
                else
                {
                    floor = floorTilesPool[floorTilesPool.Count - 1];
                    floorTilesPool.RemoveAt(floorTilesPool.Count - 1);
                    floor.transform.position = floorPosition;
                    floor.SetActive(true);
                }

                floorTiles.Add(floor);

                switch (tile.type)
                {
                    case DungeonTileType.Wall:
                    {
                        GameObject wall;
                        if (wallTilesPool.Count == 0)
                        {
                            wall = (GameObject)GameObject.Instantiate(wallPrefab, wallPosition, Quaternion.identity);
                            wall.transform.parent = trans;
                        }
                        else
                        {
                            wall = wallTilesPool[wallTilesPool.Count - 1];
                            wallTilesPool.RemoveAt(wallTilesPool.Count - 1);
                            wall.transform.position = wallPosition;
                            wall.SetActive(true);
                        }
                        wallTiles.Add(wall);
                        break;
                    }
                }
            }
        }
    }

    private void ClearDungeon()
    {
        for (int i = 0; i < floorTiles.Count; i++)
        {
            floorTilesPool.Add(floorTiles[i]);
            floorTiles[i].SetActive(false);
        }
        floorTiles.Clear();

        for (int i = 0; i < wallTiles.Count; i++)
        {
            wallTilesPool.Add(wallTiles[i]);
            wallTiles[i].SetActive(false);
        }
        wallTiles.Clear();
    }
}


