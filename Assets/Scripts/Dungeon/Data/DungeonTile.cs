using System;

public struct DungeonTile
{
    public DungeonTileType type;

    public bool visible;
	public bool walkable;
    public bool solid;

    public string floor;
    public string wallNorth;
    public string wallSouth;
    public string wallEast;
    public string wallWest;
    public string ceiling;

    static public DungeonTile WallTile
    {
        get
        {
            DungeonTile tile = new DungeonTile();

            tile.type = DungeonTileType.Wall;
            tile.solid = true;
            tile.walkable = false;
            tile.wallNorth = tile.wallSouth = tile.wallEast = tile.wallWest = "wall";
            tile.ceiling = "ceiling";

            return tile;
        }
    }

    static public DungeonTile EmptyTile
    {
        get
        {
            DungeonTile tile = new DungeonTile();
            tile.type = DungeonTileType.Empty;
            tile.solid = false;
            tile.walkable = true;
            tile.floor = "floor";

            return tile;
        }
    }
}
