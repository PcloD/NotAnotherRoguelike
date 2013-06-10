using UnityEngine;

public class RoomDecoratorUnity : MonoBehaviour, IRoomDecorator
{
    public DungeonRoomType[] roomTypes;

    private RoomDecoratorLights lightsDecorator = new RoomDecoratorLights();

    public void DecorateRoom(DungeonRoom room)
    {
        lightsDecorator.DecorateRoom(room);

        DungeonRoomType roomType = roomTypes[Random.Range(0, roomTypes.Length)];

        room.SetFloorTile(roomType.floorIds);
        room.SetWallTile(roomType.wallIds);
    }
}

