using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public int sizeX = 32;
    public int sizeY = 32;

    public DungeonUnity dungeonUnity;
    public RoomDecoratorUnity roomDecorator;

    public CameraFollowEntity cameraFollowEntity;
    public CameraMap cameraMap;

    public void Start()
    {
        RenderSettings.ambientLight = new Color32(67, 67, 67, 255);

        BuildDungeon();
    }

    public void BuildDungeon()
    {
        DungeonBuilder builder = new DungeonBuilder();

        builder.BeginDungeon();

        builder.SetSize(sizeX, sizeY);
        builder.SetGenerator(new DungeonGeneratorEmpty());
        builder.SetRoomDecorator(roomDecorator);

        builder.Build();

        Dungeon dungeon = builder.GetDungeon();

        SetDungeon(dungeon);
    }

    public void SetDungeon(Dungeon dungeon)
    {
        dungeonUnity.SetDungeon(dungeon);

        cameraFollowEntity.entity = dungeonUnity.avatar;
        cameraMap.entity = dungeonUnity.avatar;

        cameraFollowEntity.gameObject.SetActive(true);
        cameraMap.gameObject.SetActive(false);
    }

    public void OnGUI()
    {
        int size = Mathf.Max(Screen.width, Screen.height) / 10;

        if (GUI.Button(new Rect(10, 10, size * 2, size), "Rebuild Dungeon"))
            BuildDungeon();

        if (GUI.Button(new Rect(Screen.width - size - 10, Screen.height - size - 10, size, size), "Camera"))
        {
            if (cameraFollowEntity.gameObject.activeSelf)
            {
                cameraFollowEntity.gameObject.SetActive(false);
                cameraMap.gameObject.SetActive(true);
            }
            else
            {
                cameraFollowEntity.gameObject.SetActive(true);
                cameraMap.gameObject.SetActive(false);
            }
        }

        if (GUI.Button(new Rect(Screen.width - size - 10, 10, size, size), "Light"))
        {
            if (RenderSettings.ambientLight == Color.white)
                RenderSettings.ambientLight = new Color32(67, 67, 67, 255);
            else
                RenderSettings.ambientLight = Color.white;
        }
    }
}


