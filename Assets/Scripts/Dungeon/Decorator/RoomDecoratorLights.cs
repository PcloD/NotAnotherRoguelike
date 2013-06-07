using System;

public class RoomDecoratorLights : IRoomDecorator
{
    public void DecorateRoom(DungeonRoom room)
    {
        //One light in each room corner that is surrounded by walls

        if (room.GetTile(-1, 0).type == DungeonTileType.Wall &&
            room.GetTile(0, -1).type == DungeonTileType.Wall)
        {
            room.AddEntity(DungeonEntityType.Light, 0, 0, DungeonRotation.East);
        }

        if (room.GetTile(-1, room.SizeY - 1).type == DungeonTileType.Wall &&
            room.GetTile(0, room.SizeY).type == DungeonTileType.Wall)
        {
            room.AddEntity(DungeonEntityType.Light, 0, room.SizeY - 1, DungeonRotation.East);
        }

        if (room.GetTile(room.SizeX, room.SizeY - 1).type == DungeonTileType.Wall &&
            room.GetTile(room.SizeX - 1, room.SizeY).type == DungeonTileType.Wall)
        {
            room.AddEntity(DungeonEntityType.Light, room.SizeX - 1, room.SizeY - 1, DungeonRotation.West);
        }

        if (room.GetTile(room.SizeX, 0).type == DungeonTileType.Wall &&
            room.GetTile(room.SizeX - 1, -1).type == DungeonTileType.Wall)
        {
            room.AddEntity(DungeonEntityType.Light, room.SizeX - 1, 0, DungeonRotation.West);
        }
    }
}


