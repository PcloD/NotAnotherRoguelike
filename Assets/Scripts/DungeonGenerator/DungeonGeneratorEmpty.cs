using System;

public class DungeonGeneratorEmpty : IDungeonGenerator
{
    public Dungeon BuildDungeon(int sizeX, int sizeY)
    {
        Dungeon dungeon = new Dungeon();

        dungeon.Build(sizeX, sizeY, DungeonTile.WallTile);

        Random rnd = new Random();

        for (int i = 0; i < 100; i++)
        {
            int roomX = rnd.Next(1, sizeX - DungeonRoom.MIN_SIZE - 1);
            int roomY = rnd.Next(1, sizeY - DungeonRoom.MIN_SIZE - 1);

            int roomSizeX = rnd.Next(2, (sizeX - roomX) - 1);
            int roomSizeY = rnd.Next(2, (sizeY - roomY) - 1);

            if (dungeon.CheckSpaceType(roomX, roomY, roomSizeX, roomSizeY, DungeonTileType.Wall))
                dungeon.AddRoom(roomX, roomY, roomSizeX, roomSizeY);
        }

        return dungeon;
    }
}


