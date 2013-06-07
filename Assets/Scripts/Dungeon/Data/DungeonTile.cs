using System;

public struct DungeonTile
{
    public DungeonTileType type;
    public bool visible;

    static public DungeonTile WallTile
    {
        get
        {
            DungeonTile tile = new DungeonTile();
            tile.type = DungeonTileType.Wall;
            return tile;
        }
    }

    static public DungeonTile EmptyTile
    {
        get
        {
            DungeonTile tile = new DungeonTile();
            tile.type = DungeonTileType.Empty;
            return tile;
        }
    }
}
