using System;
using System.Collections.Generic;

public class DungeonMap
{
    private DungeonTile[] tiles;
    private int dungeonSizeX;
    private int dungeonSizeY;

    private List<DungeonEntity> entities = new List<DungeonEntity>();

    private DungeonVector2 startingPosition;

    private List<DungeonRoom> rooms = new List<DungeonRoom>();

    private DungeonVisibilityAlgorithm visibilityAlgorithm = new DungeonVisibilityAlgorithm();

    private IDungeonListener dungeonListener;

    private int nextEntityId;

    public void Init(int sizeX, int sizeY, DungeonTile defaultTile)
    {
        this.dungeonSizeX = sizeX;
        this.dungeonSizeY = sizeY;

        tiles = new DungeonTile[sizeX * sizeY];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = defaultTile;
    }

    public int SizeX
    {
        get { return dungeonSizeX; }
    }

    public int SizeY
    {
        get { return dungeonSizeY; }
    }

    public DungeonVector2 StartingPosition
    {
        get { return startingPosition; }
    }

    public DungeonTile GetTile(int x, int y)
    {
        return tiles[x + y * dungeonSizeX];
    }

    public void SetTile(int x, int y, DungeonTile tile)
    {
        tiles[x + y * dungeonSizeX] = tile;
    }

    public void SetTileVisible(int x, int y, bool visible)
    {
        tiles[x + y * dungeonSizeX].visible = visible;
    }

    public void SetTile(int x, int y, int sizeX, int sizeY, DungeonTile tile)
    {
        for (int xx = 0; xx < sizeX; xx++)
            for (int yy = 0; yy < sizeY; yy++)
                SetTile(xx + x, yy + y, tile);    
    }

    public DungeonRoom AddRoom(int x, int y, int sizeX, int sizeY)
    {
        if (!CheckValidBounds(x, y, sizeX, sizeY))
            throw new ArgumentException("Invalid room bounds (outside dungeon)");

        if (!CheckSpaceType(x, y, sizeX, sizeY, DungeonTileType.Wall))
            throw new ArgumentException("Invalid room bounds (can't replace non-wall tiles)");

        DungeonRoom room = new DungeonRoom(this, x, y, sizeX, sizeY);

        rooms.Add(room);

        DungeonTile roomTile = DungeonTile.EmptyTile;
        roomTile.type = DungeonTileType.Room;

        SetTile(x, y, sizeX, sizeY, roomTile);

        return room;
    }

    public int GetRoomsCount()
    {
        return rooms.Count;
    }

    public DungeonRoom GetRoom(int n)
    {
        return rooms[n];
    }

    public bool CheckValidPosition(int x, int y)
    {
        return 
            x >= 0 && x < dungeonSizeX &&
            y >= 0 && y < dungeonSizeY;
    }

    public bool CheckValidBounds(int x, int y, int sizeX, int sizeY)
    {
        return 
            x >= 0 && sizeX >= 0 && x + sizeX < dungeonSizeX &&
            y >= 0 && sizeY >= 0 && y + sizeY < dungeonSizeY;
    }

    public bool CheckSpaceType(int x, int y, int sizeX, int sizeY, DungeonTileType tileType)
    {
        if (!CheckValidBounds(x, y, sizeX, sizeY))
            throw new ArgumentException("Invalid space bounds");

        for (int xx = 0; xx < sizeX; xx++)
            for (int yy = 0; yy < sizeY; yy++)
                if (GetTile(xx + x, yy + y).type != tileType)
                    return false;

        return true;
    }

    public void SetStartingPosition(DungeonVector2 position)
    {
        startingPosition = position;
    }

    public DungeonEntity AddEntity(DungeonEntityType entityType, int x, int y, DungeonRotation rotation)
    {
        DungeonEntity entity = DungeonEntityFactory.CreateEntity(entityType);

        if (entity == null)
            throw new ArgumentException("Unknown entity type " + entityType);

        AddEntity(entity, x, y, rotation);

        return entity;
    }

    public void AddEntity(DungeonEntity entity, int x, int y, DungeonRotation rotation)
    {
        if (!CheckValidPosition(x, y))
            throw new ArgumentException("Invalid entity position");

        entities.Add(entity);

        entity.OnAddedToDungeon(this, new DungeonVector2(x, y), rotation, nextEntityId++);
    }

    public void RemoveEntity(DungeonEntity entity)
    {
        entity.OnRemovedFromDungeon();

        entities.Remove(entity);
    }

    public int GetEntitiesCount()
    {
        return entities.Count;
    }

    public DungeonEntity GetEntity(int n)
    {
        return entities[n];
    }

    public void ReportDungeonEvent(DungeonEvent dungeonEvent)
    {
        if (dungeonListener != null)
            dungeonListener.OnDungeonEvent(dungeonEvent);
    }

    public void SetDungeonListener(IDungeonListener dungeonListener)
    {
        this.dungeonListener = dungeonListener;
    }

    public void UpdateVisibility()
    {
        visibilityAlgorithm.Reset(this);
        for (int i = 0; i < GetEntitiesCount(); i++)
        {
            DungeonEntity entity = GetEntity(i);

            if (entity.Type == DungeonEntityType.Avatar)
                visibilityAlgorithm.SetVisible(this, entity.Position.x, entity.Position.y, 999);
        }
    }
}
