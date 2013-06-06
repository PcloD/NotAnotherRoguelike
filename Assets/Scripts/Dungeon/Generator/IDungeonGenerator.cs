using System;

public interface IDungeonGenerator
{
    Dungeon BuildDungeon(int sizeX, int sizeY);
}
