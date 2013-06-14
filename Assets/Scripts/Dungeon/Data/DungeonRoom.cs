using System;

//Room layout:
// 
// 1######
// #     #
// #     #
// ######2
//
// 1 -> Upper left corner in DUNGEON coordinates
// 2 -> Room size in ROOM coordinates (local sizeX, sizeY)
// # -> Wall
// (empty space) -> Floor
//
// All room functions take coordinates in ROOM space (they convert them to
// DUNGEON coordinates)
//
public class DungeonRoom
{
    public const int MIN_SIZE = 4;

    private DungeonMap dungeon;

    private int roomPositionX;
    private int roomPositionY;
    private int roomSizeX;
    private int roomSizeY;

    public int PositionX
    {
        get { return roomPositionX; }
    }

    public int PositionY
    {
        get { return roomPositionY; }
    }

    public int SizeX
    {
        get { return roomSizeX; }
    }

    public int SizeY
    {
        get { return roomSizeY; }
    }

    public DungeonMap Dungeon
    {
        get { return dungeon; }
    }

    public DungeonRoom(DungeonMap dungeon, int x, int y, int sizeX, int sizeY)
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

    public void SetFloorTile(string id)
    {
        SetFloorTile(new string[] { id });
    }

    public void SetFloorTile(string[] ids)
    {
        int idsLen = ids.Length;

        for (int y = 0; y < roomSizeY; y++)
        {
            for (int x = 0; x < roomSizeX; x++)
            {
                DungeonTile roomTile = GetTile(x, y);
                if (roomTile.type == DungeonTileType.Room)
                {
                    roomTile.floor = ids[(y + x) % idsLen];
                    SetTile(x, y, roomTile);
                }
            }
        }
    }

    public void SetWallTile(string id)
    {
        SetWallTile(new string[] { id });
    }

    public void SetWallTile(string[] ids)
    {
        //Changes the inner side of the walls surrounding the room to "id"
        //Circulates the room wall in clockwise order

        //Walls on the north (side pointing south)

        int idsLen = ids.Length;
        int idsOffset = 0;

        for (int x = 0; x < roomSizeX; x++)
        {
            DungeonTile wallTile = GetTile(x, SizeY);
            if (wallTile.type == DungeonTileType.Wall)
            {
                wallTile.wallSouth = ids[idsOffset % idsLen];
                SetTile(x, SizeY, wallTile);
            }
            idsOffset++;
        }

        //Walls on the right (side pointing west)
        for (int y = 0; y < roomSizeY; y++)
        {
            DungeonTile wallTile = GetTile(SizeX, y);
            if (wallTile.type == DungeonTileType.Wall)
            {
                wallTile.wallWest = ids[idsOffset % idsLen];
                SetTile(SizeX, y, wallTile);
            }
            idsOffset++;
        }

        //Walls on the south (side pointing north)
        for (int x = roomSizeX - 1; x >= 0; x--)
        {
            DungeonTile wallTile = GetTile(x, -1);
            if (wallTile.type == DungeonTileType.Wall)
            {
                wallTile.wallNorth = ids[idsOffset % idsLen];
                SetTile(x, -1, wallTile);
            }
            idsOffset++;
        }

        //Walls on the left (side pointing east)
        for (int y = roomSizeY - 1; y >= 0; y--)
        {
            DungeonTile wallTile = GetTile(-1, y);
            if (wallTile.type == DungeonTileType.Wall)
            {
                wallTile.wallEast = ids[idsOffset % idsLen];
                SetTile(-1, y, wallTile);
            }
            idsOffset++;
        }
    }
}


