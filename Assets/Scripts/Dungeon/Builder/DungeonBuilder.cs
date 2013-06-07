using System;

public class DungeonBuilder
{
    private Dungeon dungeon;
    private IDungeonGenerator generator;

    public void BeginDungeon(int sizeX, int sizeY)
    {
        generator = new DungeonGeneratorEmpty();

        dungeon = generator.BuildDungeon(sizeX, sizeY);
    }

    public void Decorate()
    {
        IRoomDecorator roomDecorator = new RoomDecoratorLights();

        for (int i = 0; i < dungeon.GetRoomsCount(); i++)
            roomDecorator.DecorateRoom(dungeon.GetRoom(i));
    }

    public void AddEntities()
    {
        dungeon.AddEntity(
            DungeonEntityType.Avatar, 
            dungeon.StartingPosition.x,
            dungeon.StartingPosition.y, 
            DungeonRotation.North);
    }

    public Dungeon GetDungeon()
    {
        dungeon.UpdateVisibility();

        Dungeon toReturn = dungeon;

        dungeon = null;;
        generator = null;

        return toReturn;
    }
}


