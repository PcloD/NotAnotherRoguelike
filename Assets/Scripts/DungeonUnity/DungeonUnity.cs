using UnityEngine;
using System;
using System.Collections.Generic;

public class DungeonUnity : MonoBehaviour, IDungeonListener
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    public DungeonEntityUnityFactory entityFactory;
    public DungeonEventHandlerUnityFactory eventHandlerFactory;

    public CameraFollowEntity entityCamera;

    private List<GameObject> floorTiles = new List<GameObject>();
    private List<GameObject> wallTiles = new List<GameObject>();

    private List<GameObject> floorTilesPool = new List<GameObject>();
    private List<GameObject> wallTilesPool = new List<GameObject>();

    private Dictionary<int, DungeonEntityUnity> entities = new Dictionary<int, DungeonEntityUnity>();

    private Dungeon dungeon;

    private DungeonEventQueue dungeonEventsQueue = new DungeonEventQueue();
    private DungeonEventHandlerUnity currentDungeonEventHandler;

    private DungeonEntityUnity avatarUnity;

    public void SetDungeon(Dungeon dungeon)
    {
        Clear();

        this.dungeon = dungeon;

        this.dungeon.SetDungeonListener(this);

        AddTiles();

        AddEntities();
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
            entities.Add(entity.Id, entityUnity);

            entityUnity.Init(this, entity);

            if (entity.Type == DungeonEntityType.Avatar)
            {
                avatarUnity = entityUnity;
                entityCamera.entity = avatarUnity;
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
        if (!avatarUnity)
            return;

        if (horizontal > 0 && avatarUnity.entity.CanMove(DungeonVector2.Right))
        {
            avatarUnity.entity.Rotate(DungeonRotation.East);
            avatarUnity.entity.Move(DungeonVector2.Right);
        }
        else if (horizontal < 0 && avatarUnity.entity.CanMove(DungeonVector2.Left))
        {
            avatarUnity.entity.Rotate(DungeonRotation.West);
            avatarUnity.entity.Move(DungeonVector2.Left);
        }
        else if (vertical > 0 && avatarUnity.entity.CanMove(DungeonVector2.Forward))
        {
            avatarUnity.entity.Rotate(DungeonRotation.North);
            avatarUnity.entity.Move(DungeonVector2.Forward);
        }
        else if (vertical < 0 && avatarUnity.entity.CanMove(DungeonVector2.Backward))
        {
            avatarUnity.entity.Rotate(DungeonRotation.South);
            avatarUnity.entity.Move(DungeonVector2.Backward);
        }
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
}


