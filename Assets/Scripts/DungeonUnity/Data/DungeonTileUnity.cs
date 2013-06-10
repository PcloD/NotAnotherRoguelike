using UnityEngine;
using System.Collections;

public class DungeonTileUnity : MonoBehaviour 
{
    public DungeonTileSideUnity floor;
    public DungeonTileSideUnity ceiling;
    public DungeonTileSideUnity wallNorth;
    public DungeonTileSideUnity wallSouth;
    public DungeonTileSideUnity wallEast;
    public DungeonTileSideUnity wallWest;

    public DungeonTile tile;
    public DungeonVector2 position;

    [HideInInspector]
    public DungeonUnity dungeonUnity;

    [HideInInspector]
    public Transform trans;

    [HideInInspector]
    public GameObject go;

    public void Awake()
    {
        trans = transform;
        go = gameObject;
    }
	
    public void Init(DungeonUnity dungeonUnity, DungeonVector2 position, DungeonTile tile)
    {
        this.tile = tile;
        this.position = position;
        this.dungeonUnity = dungeonUnity;

        trans.position = dungeonUnity.GetWorldPosition(position);
        trans.rotation = Quaternion.identity;

        floor = AddSide(tile.floor, DungeonTileSideType.Floor);
        ceiling = AddSide(tile.ceiling, DungeonTileSideType.Ceiling);
        wallNorth = AddSide(tile.wallNorth, DungeonTileSideType.WallNorth);
        wallSouth = AddSide(tile.wallSouth, DungeonTileSideType.WallSouth);
        wallEast = AddSide(tile.wallEast, DungeonTileSideType.WallEast);
        wallWest = AddSide(tile.wallWest, DungeonTileSideType.WallWest);
    }

    private DungeonTileSideUnity AddSide(string id, DungeonTileSideType sideType)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        if (!IsSideNonSolid(dungeonUnity.dungeon, position, sideType))
            return null;

        DungeonTileSideUnity side = dungeonUnity.tilesSideFactory.GetSide(id);

        side.trans.parent = trans;
        side.trans.localPosition = GetSidePosition(sideType);
        side.trans.localRotation = GetSideRotation(sideType);

        return side;
    }

    public void Clear()
    {
        if (!dungeonUnity)
            return;

        if (floor)
        {
            dungeonUnity.tilesSideFactory.ReturnSide(floor);
            floor = null;
        }

        if (ceiling)
        {
            dungeonUnity.tilesSideFactory.ReturnSide(ceiling);
            ceiling = null;
        }

        if (wallNorth)
        {
            dungeonUnity.tilesSideFactory.ReturnSide(wallNorth);
            wallNorth = null;
        }

        if (wallSouth)
        {
            dungeonUnity.tilesSideFactory.ReturnSide(wallSouth);
            wallSouth = null;
        }

        if (wallEast)
        {
            dungeonUnity.tilesSideFactory.ReturnSide(wallEast);
            wallEast = null;
        }

        if (wallWest)
        {
            dungeonUnity.tilesSideFactory.ReturnSide(wallWest);
            wallWest = null;
        }

        dungeonUnity = null;
    }

    public void UpdateVisiblity()
    {
        bool visible = 
            dungeonUnity.dungeon.GetTile(position.x, position.y).visible ||
            IsSideVisible(dungeonUnity.dungeon, position, DungeonTileSideType.WallEast) ||
            IsSideVisible(dungeonUnity.dungeon, position, DungeonTileSideType.WallWest) ||
            IsSideVisible(dungeonUnity.dungeon, position, DungeonTileSideType.WallNorth) ||
            IsSideVisible(dungeonUnity.dungeon, position, DungeonTileSideType.WallSouth);

        if (visible)
        {
            if (floor)
                floor.go.layer = LayerConstants.LAYER_SIDE_LIGHT;

            if (ceiling)
                ceiling.go.layer = LayerConstants.LAYER_SIDE_LIGHT;

            if (wallNorth)
                wallNorth.go.layer = LayerConstants.LAYER_SIDE_LIGHT;

            if (wallSouth)
                wallSouth.go.layer = LayerConstants.LAYER_SIDE_LIGHT;

            if (wallEast)
                wallEast.go.layer = LayerConstants.LAYER_SIDE_LIGHT;

            if (wallWest)
                wallWest.go.layer = LayerConstants.LAYER_SIDE_LIGHT;
        }
        else
        {
            if (floor)
                floor.go.layer = LayerConstants.LAYER_SIDE_NO_LIGHT;

            if (ceiling)
                ceiling.go.layer = LayerConstants.LAYER_SIDE_NO_LIGHT;

            if (wallNorth)
                wallNorth.go.layer = LayerConstants.LAYER_SIDE_NO_LIGHT;

            if (wallSouth)
                wallSouth.go.layer = LayerConstants.LAYER_SIDE_NO_LIGHT;

            if (wallEast)
                wallEast.go.layer = LayerConstants.LAYER_SIDE_NO_LIGHT;

            if (wallWest)
                wallWest.go.layer = LayerConstants.LAYER_SIDE_NO_LIGHT;
        }
    }

    static public bool IsSideVisible(Dungeon dungeon, DungeonVector2 position, DungeonTileSideType sideType)
    {
        switch (sideType)
        {
            case DungeonTileSideType.WallSouth:
                if (position.y > 0 &&
                    !dungeon.GetTile(position.x, position.y - 1).visible)
                {
                    return false;
                }
                break;

            case DungeonTileSideType.WallNorth:
                if (position.y + 1 < dungeon.SizeY &&
                    !dungeon.GetTile(position.x, position.y + 1).visible)
                {
                    return false;
                }
                break;

            case DungeonTileSideType.WallWest:
                if (position.x > 0 &&
                    !dungeon.GetTile(position.x - 1, position.y).visible)
                {
                    return false;
                }
                break;

            case DungeonTileSideType.WallEast:
                if (position.x + 1 < dungeon.SizeX &&
                    !dungeon.GetTile(position.x + 1, position.y).visible)
                {
                    return false;
                }
                break;
        }

        return true;
    }
    static public bool IsSideNonSolid(Dungeon dungeon, DungeonVector2 position, DungeonTileSideType sideType)
    {
        switch (sideType)
        {
            case DungeonTileSideType.WallSouth:
                if (position.y > 0 &&
                    dungeon.GetTile(position.x, position.y - 1).solid)
                {
                    return false;
                }
                break;

            case DungeonTileSideType.WallNorth:
                if (position.y + 1 < dungeon.SizeY &&
                    dungeon.GetTile(position.x, position.y + 1).solid)
                {
                    return false;
                }
                break;

            case DungeonTileSideType.WallWest:
                if (position.x > 0 &&
                    dungeon.GetTile(position.x - 1, position.y).solid)
                {
                    return false;
                }
                break;

            case DungeonTileSideType.WallEast:
                if (position.x + 1 < dungeon.SizeX &&
                    dungeon.GetTile(position.x + 1, position.y).solid)
                {
                    return false;
                }
                break;
        }

        return true;
    }

    static public Vector3 GetSidePosition(DungeonTileSideType side)
    {
        switch (side)
        {
            case DungeonTileSideType.Ceiling:
                return new Vector3(0.0f, 1.0f, 0.0f);
            case DungeonTileSideType.Floor:
                return new Vector3(0.0f, 0.0f, 0.0f);
            case DungeonTileSideType.WallNorth:
                return new Vector3(0.0f, 0.5f, 0.5f);
            case DungeonTileSideType.WallSouth:
                return new Vector3(0.0f, 0.5f, -0.5f);
            case DungeonTileSideType.WallEast:
                return new Vector3(0.5f, 0.5f, 0.0f);
            case DungeonTileSideType.WallWest:
                return new Vector3(-0.5f, 0.5f, 0.0f);
            default:
                return Vector3.zero;
        }
    }

    static public Quaternion GetSideRotation(DungeonTileSideType side)
    {
        switch (side)
        {
            case DungeonTileSideType.Ceiling:
                return Quaternion.Euler(0.0f, 0.0f, 0.0f);
            case DungeonTileSideType.Floor:
                return Quaternion.Euler(0.0f, 0.0f, 0.0f);
            case DungeonTileSideType.WallNorth:
                return Quaternion.Euler(90.0f, 0.0f, 0.0f);
            case DungeonTileSideType.WallSouth:
                return Quaternion.Euler(-90.0f, 0.0f, 0.0f);
            case DungeonTileSideType.WallEast:
                return Quaternion.Euler(0.0f, 0.0f, -90.0f);
            case DungeonTileSideType.WallWest:
                return Quaternion.Euler(0.0f, 0.0f, 90.0f);
            default:
                return Quaternion.identity;
        }
    }
}
