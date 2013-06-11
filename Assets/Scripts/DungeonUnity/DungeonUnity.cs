using UnityEngine;
using System;
using System.Collections.Generic;

public class DungeonUnity : MonoBehaviour, IDungeonListener
{
    public DungeonEntityUnityFactory entityFactory;
    public DungeonEventHandlerUnityFactory eventHandlerFactory;
    public DungeonTileUnityPool tilesPool;
    public DungeonTileSideUnityFactory tilesSideFactory;
    public DungeonShadowUnityPool shadowTilesPool;

    public Dungeon dungeon;

    private DungeonTileUnity[] tiles;
    private DungeonShadowUnity[] shadowTiles;

    private Transform shadowTilesContainer;
    private Transform tilesContainer;
    private Transform entitiesContainer;

    private Dictionary<int, DungeonEntityUnity> entities = new Dictionary<int, DungeonEntityUnity>();

    private DungeonEventQueue dungeonEventsQueue = new DungeonEventQueue();
    private DungeonEventHandlerUnity currentDungeonEventHandler;

    [HideInInspector]
    public DungeonEntityAvatarUnity avatar;

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

        //Reset dungeon container to default position befor initializing
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

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
                avatar = (DungeonEntityAvatarUnity) entityUnity;
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
                if (tiles[i])
                    tilesPool.ReturnTile(tiles[i]);
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
                if (shadowTiles[i])
                    shadowTilesPool.ReturnShadowTile(shadowTiles[i]);

            shadowTiles = null;
        }

        currentDungeonEventHandler = null;

        if (dungeon != null)
        {
            dungeon.SetDungeonListener(null);
            dungeon = null;
        }
    }

    public bool IsBusy()
    {
        return currentDungeonEventHandler != null || !dungeonEventsQueue.IsEmpty;
    }

    public void Update()
    {
        UpdateEventsQueue();
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

    public void UpdateVisibility()
    {
        dungeon.UpdateVisibility();

        if (shadowTiles == null)
            shadowTiles = new DungeonShadowUnity[dungeon.SizeX * dungeon.SizeY];

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
                        //Shadow active in visible tile, remove it!
                        shadowTilesPool.ReturnShadowTile(shadowTiles[tileIndex]);
                        shadowTiles[tileIndex] = null;
                    }
                }
                else
                {
                    if (!shadowTiles[tileIndex])
                    {
                        //No shadow in invisible tile, add it
                        shadowTiles[tileIndex] = shadowTilesPool.GetShadowTile();
                        shadowTiles[tileIndex].trans.parent = shadowTilesContainer;
                        shadowTiles[tileIndex].trans.position = GetWorldPosition(x, y);
                    }
                }
            }
        }
    }
}


