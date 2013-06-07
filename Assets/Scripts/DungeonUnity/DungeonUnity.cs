using UnityEngine;
using System;
using System.Collections.Generic;

public class DungeonUnity : MonoBehaviour, IDungeonListener
{
    public GameObject shadowPrefab;
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    public DungeonEntityUnityFactory entityFactory;
    public DungeonEventHandlerUnityFactory eventHandlerFactory;

    public CameraFollowEntity entityCamera;

    private List<GameObject> floorTiles = new List<GameObject>();
    private List<GameObject> wallTiles = new List<GameObject>();
    private GameObject[] shadowTiles;

    private List<GameObject> floorTilesPool = new List<GameObject>();
    private List<GameObject> wallTilesPool = new List<GameObject>();
    private List<GameObject> shadowTilesPool = new List<GameObject>();

    private Transform shadowTilesContainer;
    private Transform floorTilesContainer;
    private Transform wallTilesContainer;
    private Transform entitiesContainer;

    private Dictionary<int, DungeonEntityUnity> entities = new Dictionary<int, DungeonEntityUnity>();

    private Dungeon dungeon;

    private DungeonEventQueue dungeonEventsQueue = new DungeonEventQueue();
    private DungeonEventHandlerUnity currentDungeonEventHandler;

    private DungeonEntityAvatar avatar;

    public void Awake()
    {
        shadowTilesContainer = new GameObject("ShadowTiles").transform;
        shadowTilesContainer.parent = transform;
        shadowTilesContainer.localPosition = Vector3.zero;
        shadowTilesContainer.localRotation = Quaternion.identity;

        floorTilesContainer = new GameObject("FloorTiles").transform;
        floorTilesContainer.parent = transform;
        floorTilesContainer.localPosition = Vector3.zero;
        floorTilesContainer.localRotation = Quaternion.identity;

        wallTilesContainer = new GameObject("WallTiles").transform;
        wallTilesContainer.parent = transform;
        wallTilesContainer.localPosition = Vector3.zero;
        wallTilesContainer.localRotation = Quaternion.identity;

        entitiesContainer = new GameObject("Entities").transform;
        entitiesContainer.parent = transform;
        entitiesContainer.localPosition = Vector3.zero;
        entitiesContainer.localRotation = Quaternion.identity;
    }

    public void SetDungeon(Dungeon dungeon)
    {
        Clear();

        this.dungeon = dungeon;

        this.dungeon.SetDungeonListener(this);

        AddTiles();

        AddEntities();

        UpdateVisibility();
    }

    private void AddEntities()
    {
        for (int i = 0; i < dungeon.GetEntitiesCount(); i++)
            AddEntity(dungeon.GetEntity(i));
    }

    private void AddEntity(DungeonEntity entity)
    {
        DungeonEntityUnity entityUnity = entityFactory.CreateEntity(entity);

        if (entityUnity != null)
        {
            entityUnity.trans.parent = entitiesContainer;

            entities.Add(entity.Id, entityUnity);

            entityUnity.Init(this, entity);

            if (entity.Type == DungeonEntityType.Avatar)
            {
                avatar = (DungeonEntityAvatar) entityUnity.entity;
                entityCamera.entity = entityUnity;
            }
        }
        else
        {
            Debug.LogWarning("No unity entity created for entity type: " + entity.Type);
        }
    }

    public DungeonEntityUnity GetEntityUnity(DungeonEntity entity)
    {
        if (entities.ContainsKey(entity.Id))
            return entities[entity.Id];

        return null;
    }

    private void AddTiles()
    {
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
                    floor.transform.parent = floorTilesContainer;
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
                            wall.transform.parent = wallTilesContainer;
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

    private void Clear()
    {
        //Destroy floor
        for (int i = 0; i < floorTiles.Count; i++)
        {
            floorTilesPool.Add(floorTiles[i]);
            floorTiles[i].SetActive(false);
        }
        floorTiles.Clear();

        //Destroy tiles
        for (int i = 0; i < wallTiles.Count; i++)
        {
            wallTilesPool.Add(wallTiles[i]);
            wallTiles[i].SetActive(false);
        }
        wallTiles.Clear();

        //Destroy entities
        for (int i = 0; i < entities.Count; i++)
            entityFactory.DestroyEntity(entities[i]);
        entities.Clear();

        //Clear events queue
        dungeonEventsQueue.Clear();

        if (shadowTiles != null)
        {
            for (int i = 0; i < shadowTiles.Length; i++)
            {
                if (shadowTiles[i])
                {
                    shadowTilesPool.Add(shadowTiles[i]);
                    shadowTiles[i].SetActive(false);
                }
            }

            shadowTiles = null;
        }

        currentDungeonEventHandler = null;

        dungeon = null;
    }

    public bool IsBusy()
    {
        return currentDungeonEventHandler != null || !dungeonEventsQueue.IsEmpty;
    }

    public void Update()
    {
        UpdateEventsQueue();

        UpdatePlayerInput();
    }

    private void UpdatePlayerInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        MoveAvatar(horizontal, vertical);
    }

    public void OnGUI()
    {
        float bottom = Screen.height - 10;
        float left = 10;
        float size = (int) (Mathf.Max(Screen.width, Screen.height) / 10);
        float space = size / 6;

        if (GUI.RepeatButton(new Rect(left, bottom - size, size, size), "Left"))
            MoveAvatar(-1, 0);

        if (GUI.RepeatButton(new Rect(left + (size + space) * 1, bottom - size, size, size), "Down"))
            MoveAvatar(0, -1);

        if (GUI.RepeatButton(new Rect(left + (size + space) * 2, bottom - size, size, size), "Right"))
            MoveAvatar(1, 0);

        if (GUI.RepeatButton(new Rect(left + (size + space) * 1, bottom - (size + space) * 1 - size, size, size), "Up"))
            MoveAvatar(0, 1);
    }

    private void MoveAvatar(float horizontal, float vertical)
    {
        //No input handling while the dungeon is updating
        if (IsBusy())
            return;

        //No input handling if no avatar is available
        if (avatar == null)
            return;

        if (horizontal == 0 && vertical == 0)
            return;

        if (horizontal > 0)
            avatar.Walk(DungeonVector2.Right);
        else if (horizontal < 0)
            avatar.Walk(DungeonVector2.Left);
        else if (vertical > 0)
            avatar.Walk(DungeonVector2.Forward);
        else if (vertical < 0)
            avatar.Walk(DungeonVector2.Back);

        dungeon.UpdateVisibility();

        UpdateVisibility();
    }

    private void UpdateEventsQueue()
    {
        if (currentDungeonEventHandler == null)
        {
            if (!dungeonEventsQueue.IsEmpty)
            {
                DungeonEvent dungeonEvent = dungeonEventsQueue.DequeueEvent();

                currentDungeonEventHandler = eventHandlerFactory.CreateEventHandler(this, dungeonEvent);
            }
        }

        if (currentDungeonEventHandler != null)
        {
            if (currentDungeonEventHandler.Update())
                currentDungeonEventHandler = null;
        }
    }

    public void OnDungeonEvent(DungeonEvent dungeonEvent)
    {
        dungeonEventsQueue.EnqueueEvent(dungeonEvent);
    }

    public Vector3 GetWorldPosition(DungeonVector2 position)
    {
        return new Vector3(position.x, 0, position.y);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y);
    }

    public Quaternion GetWorldRotation(DungeonRotation rotation)
    {
        switch (rotation)
        {
            case DungeonRotation.North:
                return Quaternion.LookRotation(Vector3.forward);
            case DungeonRotation.South:
                return Quaternion.LookRotation(Vector3.back);
            case DungeonRotation.East:
                return Quaternion.LookRotation(Vector3.right);
            case DungeonRotation.West:
                return Quaternion.LookRotation(Vector3.left);
        }

        return Quaternion.identity;
    }

    private void UpdateVisibility()
    {
        if (shadowTiles == null)
            shadowTiles = new GameObject[dungeon.SizeX * dungeon.SizeY];

        for (int x = 0; x < dungeon.SizeX; x++)
        {
            for (int y = 0; y < dungeon.SizeY; y++)
            {
                int shadowTileIndex = x + y * dungeon.SizeX;

                DungeonTile tile = dungeon.GetTile(x, y);

                if (tile.visible || tile.type == DungeonTileType.Wall)
                {
                    if (shadowTiles[shadowTileIndex])
                    {
                        //Shadow active in newly visible tile, remove it!
                        shadowTiles[shadowTileIndex].SetActive(false);
                        shadowTilesPool.Add(shadowTiles[shadowTileIndex]);
                        shadowTiles[shadowTileIndex] = null;
                    }
                }
                else
                {
                    if (!shadowTiles[shadowTileIndex])
                    {
                        //No shadow in invisible tile, add it
                        if (shadowTilesPool.Count > 0)
                        {
                            shadowTiles[shadowTileIndex] = shadowTilesPool[shadowTilesPool.Count - 1];
                            shadowTilesPool.RemoveAt(shadowTilesPool.Count - 1);

                            shadowTiles[shadowTileIndex].SetActive(true);
                        }
                        else
                        {
                            shadowTiles[shadowTileIndex] = (GameObject)GameObject.Instantiate(shadowPrefab);
                            shadowTiles[shadowTileIndex].transform.parent = shadowTilesContainer;
                        }

                        shadowTiles[shadowTileIndex].transform.position = GetWorldPosition(x, y);
                    }
                }
            }
        }
    }
}


