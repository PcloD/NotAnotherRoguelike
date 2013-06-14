using System;

public interface IDungeonGenerator
{
    DungeonMap BuildDungeon(int sizeX, int sizeY);
}
