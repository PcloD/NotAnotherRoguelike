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

    public void AddEntities()
    {
        dungeon.AddEntity(
            new DungeonEntity(DungeonEntityType.Avatar), 
            dungeon.StartingPosition.x,
            dungeon.StartingPosition.y);
    }

    public Dungeon GetDungeon()
    {
        Dungeon toReturn = dungeon;

        dungeon = null;;
        generator = null;

        return toReturn;
    }
}


