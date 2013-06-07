using System;

public class DungeonRoom
{
    public const int MIN_SIZE = 4;

    private Dungeon dungeon;

    private int roomPositionX;
    private int roomPositionY;
    private int roomSizeX;
    private int roomSizeY;

    public int SizeX
    {
        get { return roomSizeX; }
    }

    public int SizeY
    {
        get { return roomSizeY; }
    }

    public Dungeon Dungeon
    {
        get { return dungeon; }
    }

    public DungeonRoom(Dungeon dungeon, int x, int y, int sizeX, int sizeY)
    {
        this.dungeon = dungeon;

        this.roomPositionX = x;
        this.roomPositionY = y;
        this.roomSizeX = sizeX;
        this.roomSizeY = sizeY;
    }

    public DungeonTile GetTile(int x, int y)
    {
        //if (!CheckValidPosition(x, y))
        //    throw new ArgumentException("Invalid tile position in room");

        return dungeon.GetTile(x + roomPositionX, y + roomPositionY);
    }

    public void SetTile(int x, int y, DungeonTile tile)
    {
        //if (!CheckValidPosition(x, y))
       //     throw new ArgumentException("Invalid tile position in room");

        dungeon.SetTile(x + roomPositionX, y + roomPositionY, tile);
    }

    public void SetTile(int x, int y, int sizeX, int sizeY, DungeonTile tile)
    {
        //if (!CheckValidBounds(x, y, sizeX, sizeY))
        //    throw new ArgumentException("Invalid tile position in room");

        dungeon.SetTile(x + roomPositionX, y + roomPositionY, sizeX, sizeY, tile);
    }

    public bool CheckValidPosition(int x, int y)
    {
        return 
            x >= 0 && x < roomSizeX &&
            y >= 0 && y < roomSizeY;
    }

    public bool CheckValidBounds(int x, int y, int sizeX, int sizeY)
    {
        return 
            x >= 0 && sizeX >= 0 && x + sizeX < roomSizeX &&
            y >= 0 && sizeY >= 0 && y + sizeY < roomSizeY;
    }

    public void AddEntity(DungeonEntityType entityType, int x, int y, DungeonRotation rotation)
    {
        //if (!CheckValidPosition(x, y))
        //    throw new ArgumentException("Invalid entity position in room");

        dungeon.AddEntity(entityType, roomPositionX + x, roomPositionY + y, rotation);
    }

    
    public bool CheckSpaceType(int x, int y, int sizeX, int sizeY, DungeonTileType tileType)
    {
        //if (!CheckValidBounds(x, y, sizeX, sizeY))
        //    throw new ArgumentException("Invalid space bounds in room");

        return dungeon.CheckSpaceType(x + roomPositionX, y + roomPositionY, sizeX, sizeY, tileType);
    }
}


