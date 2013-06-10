using UnityEngine;
using System;
using System.Collections.Generic;

public class DungeonUnity : MonoBehaviour, IDungeonListener
{
    public GameObject shadowPrefab;

    public DungeonEntityUnityFactory entityFactory;
    public DungeonEventHandlerUnityFactory eventHandlerFactory;
    public DungeonTileUnityPool tilesPool;
    public DungeonTileSideUnityFactory tilesSideFactory;

    public CameraFollowEntity entityCamera;

    private DungeonTileUnity[] tiles;

    private List<GameObject> shadowTilesPool = new List<GameObject>();
    private GameObject[] shadowTiles;

    private Transform shadowTilesContainer;
    private Transform tilesContainer;
    private Transform entitiesContainer;

    private Dictionary<int, DungeonEntityUnity> entities = new Dictionary<int, DungeonEntityUnity>();

    public Dungeon dungeon;

    private DungeonEventQueue dungeonEventsQueue = new DungeonEventQueue();
    private DungeonEventHandlerUnity currentDungeonEventHandler;

    private DungeonEntityAvatar avatar;

    public void Awake()
    {
        shadowTilesContainer = new GameObject("ShadowTiles").transform;
        shadowTilesContainer.parent = transform;
        shadowTilesContainer.localPosition = Vector3.zero;
        shadowTilesContainer.localRotation = Quaternion.identity;

        tilesContainer = new GameObject("Tiles").transform;
        tilesContainer.parent = transform;
        tilesContainer.localPosition = Vector3.zero;
        tilesContainer.localRotation = Quaternion.identity;

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

        tiles = new DungeonTileUnity[dungeon.SizeX * dungeon.SizeY];

        for (int x = 0; x < dungeon.SizeX; x++)
        {
            for (int y = 0; y < dungeon.SizeY; y++)
            {
                DungeonTile tile = dungeon.GetTile(x, y);

                DungeonTileUnity tileUnity = tilesPool.GetTile();

                tileUnity.transform.parent = tilesContainer;

                tileUnity.Init(this, new DungeonVector2(x, y), tile);

                tiles[x + y * dungeon.SizeX] = tileUnity;
            }
        }
    }

    private void Clear()
    {
        //Remove tiles
        if (tiles != null)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i])
                    tilesPool.ReturnTile(tiles[i]);
            }
            tiles = null;
        }

        //Remove entities
        for (int i = 0; i < entities.Count; i++)
            entityFactory.DestroyEntity(entities[i]);
        entities.Clear();

        //Clear events queue
        dungeonEventsQueue.Clear();

        //Remove shadow tiles
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
                int tileIndex = x + y * dungeon.SizeX;

                DungeonTile tile = dungeon.GetTile(x, y);

                tiles[tileIndex].UpdateVisiblity();

                if (tile.visible || tile.solid)
                {
                    if (shadowTiles[tileIndex])
                    {
                        //Shadow active in newly visible tile, remove it!
                        shadowTiles[tileIndex].SetActive(false);
                        shadowTilesPool.Add(shadowTiles[tileIndex]);
                        shadowTiles[tileIndex] = null;
                    }
                }
                else
                {
                    if (!shadowTiles[tileIndex])
                    {
                        //No shadow in invisible tile, add it
                        if (shadowTilesPool.Count > 0)
                        {
                            shadowTiles[tileIndex] = shadowTilesPool[shadowTilesPool.Count - 1];
                            shadowTilesPool.RemoveAt(shadowTilesPool.Count - 1);

                            shadowTiles[tileIndex].SetActive(true);
                        }
                        else
                        {
                            shadowTiles[tileIndex] = (GameObject)GameObject.Instantiate(shadowPrefab);
                            shadowTiles[tileIndex].transform.parent = shadowTilesContainer;
                        }

                        shadowTiles[tileIndex].transform.position = GetWorldPosition(x, y);
                    }
                }
            }
        }
    }
}


