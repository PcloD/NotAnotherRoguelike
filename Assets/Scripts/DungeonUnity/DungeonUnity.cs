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

    public DungeonMap dungeon;

    private DungeonTileUnity[] tiles;
    private DungeonShadowUnity[] shadowTiles;

    private Transform shadowTilesContainer;
    private Transform tilesContainer;
    private Transform entitiesContainer;
    private Transform combinedTilesContainer;

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

        combinedTilesContainer = new GameObject("CombinedTiles").transform;
        combinedTilesContainer.parent = transform;
        combinedTilesContainer.localPosition = Vector3.zero;
        combinedTilesContainer.localRotation = Quaternion.identity;
    }

    public void SetDungeon(DungeonMap dungeon)
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

        //Create tiles
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

        //Combine tiles
        CombineTiles(8);

        //Return tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i])
            {
                tilesPool.ReturnTile(tiles[i]);
                tiles[i] = null;
            }
        }
    }

    private List<MeshFilter> tmpMeshes = new List<MeshFilter>(1024);
    private Dictionary<Material, List<MeshFilter>> tmpMeshesByMaterial = new Dictionary<Material, List<MeshFilter>>();

    private void CombineTiles(int groupSize)
    {
        for (int gx = 0; gx < dungeon.SizeX; gx += groupSize)
        {
            for (int gy = 0; gy < dungeon.SizeY; gy += groupSize)
            {
                GameObject groupGO = new GameObject("Group-" + gx + "-" + gy);
                groupGO.transform.parent = combinedTilesContainer;
                groupGO.transform.localPosition = Vector3.zero;
                groupGO.transform.localRotation = Quaternion.identity;

                //Get all the mesh filters in the subgroup
                tmpMeshes.Clear();

                for (int x = gx; x < gx + groupSize; x++)
                {
                    for (int y = gy; y < gy + groupSize; y++)
                    {
                        DungeonTileUnity tile = tiles[x + y * dungeon.SizeY];

                        if (tile)
                        {
                            if (tile.ceiling)
                                tmpMeshes.Add(tile.ceiling.GetComponent<MeshFilter>());
                            if (tile.floor)
                                tmpMeshes.Add(tile.floor.GetComponent<MeshFilter>());
                            if (tile.wallEast)
                                tmpMeshes.Add(tile.wallEast.GetComponent<MeshFilter>());
                            if (tile.wallNorth)
                                tmpMeshes.Add(tile.wallNorth.GetComponent<MeshFilter>());
                            if (tile.wallSouth)
                                tmpMeshes.Add(tile.wallSouth.GetComponent<MeshFilter>());
                            if (tile.wallWest)
                                tmpMeshes.Add(tile.wallWest.GetComponent<MeshFilter>());
                        }
                    }
                }

                //Group meshes by material
                tmpMeshesByMaterial.Clear();

                for (int i = 0; i < tmpMeshes.Count; i++)
                {
                    Material material = tmpMeshes[i].renderer.sharedMaterial;

                    if (!tmpMeshesByMaterial.ContainsKey(material))
                        tmpMeshesByMaterial[material] = new List<MeshFilter>();

                    tmpMeshesByMaterial[material].Add(tmpMeshes[i]);
                }

                //Combine meshes by material
                foreach (Material material in tmpMeshesByMaterial.Keys)
                {
                    List<MeshFilter> materialMeshes = tmpMeshesByMaterial[material];

                    CombineInstance[] combine = new CombineInstance[materialMeshes.Count];

                    for (int i = 0; i < materialMeshes.Count; i++)
                    {
                        //Add combine info
                        combine[i].mesh = materialMeshes[i].sharedMesh;
                        combine[i].transform = materialMeshes[i].transform.localToWorldMatrix;
                    }

                    //Combine mesh
                    Mesh combinedMesh = new Mesh();
                    combinedMesh.CombineMeshes(combine);

                    //Create game object with MesFilter / MeshRenderer to draw the combined mesh
                    GameObject combinedMeshGO = new GameObject("Combined-" + material.name);

                    combinedMeshGO.transform.parent = groupGO.transform;
                    combinedMeshGO.transform.localPosition = Vector3.zero;
                    combinedMeshGO.transform.localRotation = Quaternion.identity;

                    MeshFilter combinedMeshFilter = combinedMeshGO.AddComponent<MeshFilter>();
                    MeshRenderer combinedMeshRenderer = combinedMeshGO.AddComponent<MeshRenderer>();

                    combinedMeshFilter.sharedMesh = combinedMesh;
                    combinedMeshRenderer.sharedMaterial = material;
                }
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

        for (int i = combinedTilesContainer.childCount - 1; i >= 0; i--)
            GameObject.Destroy(combinedTilesContainer.GetChild(i).gameObject);
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

                if (tiles[tileIndex])
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


