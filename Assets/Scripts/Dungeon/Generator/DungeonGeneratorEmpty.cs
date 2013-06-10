using System;

public class DungeonGeneratorEmpty : IDungeonGenerator
{
    public Dungeon BuildDungeon(int sizeX, int sizeY)
    {
        Dungeon dungeon = new Dungeon();

        dungeon.Init(sizeX, sizeY, DungeonTile.WallTile);

        Random rnd = new Random();

        //Add random rooms
        for (int i = 0; i < 100; i++)
        {
            int roomX = rnd.Next(1, sizeX - DungeonRoom.MIN_SIZE - 1);
            int roomY = rnd.Next(1, sizeY - DungeonRoom.MIN_SIZE - 1);

            int roomSizeX = rnd.Next(DungeonRoom.MIN_SIZE, (sizeX - roomX) - 1);
            int roomSizeY = rnd.Next(DungeonRoom.MIN_SIZE, (sizeY - roomY) - 1);

            if (dungeon.CheckSpaceType(roomX - 1, roomY - 1, roomSizeX + 2, roomSizeY + 2, DungeonTileType.Wall))
            {
                dungeon.AddRoom(roomX, roomY, roomSizeX, roomSizeY);

                dungeon.SetStartingPosition(new DungeonVector2(roomX + DungeonRoom.MIN_SIZE / 2, roomY +  + DungeonRoom.MIN_SIZE / 2));
            }
        }

        //Connect all the rooms! (room0 -> room1, room1 -> room2, etc..)
        for (int i = 0; i < dungeon.GetRoomsCount() - 1; i++)
        {
            DungeonRoom fromRoom = dungeon.GetRoom(i);
            DungeonRoom toRoom = dungeon.GetRoom(i + 1);

            int fromX = fromRoom.PositionX + fromRoom.SizeX / 2;
            int fromY = fromRoom.PositionY + fromRoom.SizeY / 2;

            int toX = toRoom.PositionX + toRoom.SizeX / 2;
            int toY = toRoom.PositionY + toRoom.SizeY / 2;

            if (toX != fromX)
            {
                int stepsX = Math.Abs(toX - fromX);
                int dx = (toX - fromX) / stepsX;

                for (int sx = 0; sx < stepsX; sx++)
                {
                    int x = fromX + sx * dx;
                    int y = fromY;

                    if (dungeon.GetTile(x, y).type == DungeonTileType.Wall)
                        dungeon.SetTile(x, y, DungeonTile.EmptyTile);
                }
            }

            if (toY != fromY)
            {
                int stepsY = Math.Abs(toY - fromY);
                int dy = (toY - fromY) / stepsY;

                for (int sy = 0; sy < stepsY; sy++)
                {
                    int x = toX;
                    int y = fromY + sy * dy;

                    if (dungeon.GetTile(x, y).type == DungeonTileType.Wall)
                        dungeon.SetTile(x, y, DungeonTile.EmptyTile);
                }
            }
        }

        return dungeon;
    }
}


