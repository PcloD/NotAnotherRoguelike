using System;

public class DungeonBuilder
{
    private DungeonMap dungeon;
    private IDungeonGenerator generator;
    private IRoomDecorator roomDecorator;

    private int sizeX;
    private int sizeY;

    public void BeginDungeon()
    {
    }

    public void SetSize(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
    }

    public void SetGenerator(IDungeonGenerator generator)
    {
        this.generator = generator;
    }

    public void SetRoomDecorator(IRoomDecorator roomDecorator)
    {
        this.roomDecorator = roomDecorator;
    }

    public void Build()
    {
        dungeon = generator.BuildDungeon(sizeX, sizeY);

        for (int i = 0; i < dungeon.GetRoomsCount(); i++)
            roomDecorator.DecorateRoom(dungeon.GetRoom(i));

        dungeon.AddEntity(
            DungeonEntityType.Avatar, 
            dungeon.StartingPosition.x,
            dungeon.StartingPosition.y, 
            DungeonRotation.North);
    }

    public DungeonMap GetDungeon()
    {
        dungeon.UpdateVisibility();

        DungeonMap toReturn = dungeon;

        dungeon = null;
        generator = null;
        roomDecorator = null;
        sizeX = sizeY = 0;

        return toReturn;
    }
}


