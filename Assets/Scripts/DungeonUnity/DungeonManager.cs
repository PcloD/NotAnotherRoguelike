using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private const float MOVE_CAMERA_TIME = 1.0f;
    private const float COLLAPSE_TIME = 1.0f;
    private const float ROTATE_PAGE_TIME = 1.0f;

    private enum BuildingDungeonNiceState
    {
        MOVING_CAMERA_AWAY,
        COLLAPSING_OLD,
        CREATING_NEW,
        TURNING_PAGE,
        EXPANDING_NEW,
        MOVING_CAMERA_IN
    }

    public int sizeX = 32;
    public int sizeY = 32;

    public DungeonBook dungeonBook;

    public DungeonUnity dungeonUnity;
    public RoomDecoratorUnity roomDecorator;

    public CameraFollowEntity cameraFollowEntity;
    public CameraMap cameraMap;

    private bool buildingDungeonNice;
    private float buildingDungeonNiceTime;
    private BuildingDungeonNiceState buildingDungeonNiceState;

    public void Start()
    {
        RenderSettings.ambientLight = new Color32(67, 67, 67, 255);

        BuildDungeon(true);
    }

    public void BuildDungeon(bool updateCamera)
    {
        DungeonBuilder builder = new DungeonBuilder();

        builder.BeginDungeon();

        builder.SetSize(sizeX, sizeY);
        builder.SetGenerator(new DungeonGeneratorEmpty());
        builder.SetRoomDecorator(roomDecorator);

        builder.Build();

        Dungeon dungeon = builder.GetDungeon();

        SetDungeon(dungeon, updateCamera);
    }

    public void SetDungeon(Dungeon dungeon, bool updateCamera = true)
    {
        dungeonUnity.SetDungeon(dungeon);

        if (updateCamera)
        {
            cameraFollowEntity.entity = dungeonUnity.avatar;
            cameraMap.entity = dungeonUnity.avatar;

            cameraFollowEntity.gameObject.SetActive(true);
            cameraMap.gameObject.SetActive(false);

            dungeonBook.DrawOldDungeon(dungeon);
        }
    }

    public void OnGUI()
    {
        if (buildingDungeonNice)
            return;

        int size = Mathf.Max(Screen.width, Screen.height) / 10;

        if (GUI.Button(new Rect(10, 10, size, size), "New"))
            BuildDungeon(true);

        if (GUI.Button(new Rect(10 + size + 10, 10, size, size), "New Nice"))
        {
            BuildDungeonNice();
        }

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

    private void BuildDungeonNice()
    {
        buildingDungeonNice = true;

        cameraFollowEntity.gameObject.SetActive(true);
        cameraMap.gameObject.SetActive(false);

        cameraFollowEntity.entity = null;

        SwitchToState(BuildingDungeonNiceState.MOVING_CAMERA_AWAY);

    }

    public void Update()
    {
        if (buildingDungeonNice)
        {
            switch (buildingDungeonNiceState)
            {
                case BuildingDungeonNiceState.MOVING_CAMERA_AWAY:
                    if (buildingDungeonNiceTime > MOVE_CAMERA_TIME)
                        SwitchToState(BuildingDungeonNiceState.COLLAPSING_OLD);
                    break;

                case BuildingDungeonNiceState.COLLAPSING_OLD:
                    dungeonUnity.transform.localScale = new Vector3(1.0f, Mathf.Max(0.0f, 1.0f - buildingDungeonNiceTime / COLLAPSE_TIME), 1.0f);
                    dungeonUnity.transform.position = new Vector3(0.0f, -buildingDungeonNiceTime / COLLAPSE_TIME, 0.0f);
                    if (dungeonUnity.transform.localScale.y == 0.0f)
                    {
                        dungeonUnity.gameObject.SetActive(false);
                        SwitchToState(BuildingDungeonNiceState.CREATING_NEW);
                    }
                    break;

                case BuildingDungeonNiceState.CREATING_NEW:
                    dungeonUnity.gameObject.SetActive(true);
                    dungeonUnity.transform.localScale = Vector3.one;
                    dungeonUnity.transform.position = Vector3.zero;

                    BuildDungeon(false);

                    dungeonUnity.gameObject.SetActive(false);
                    dungeonUnity.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
                    dungeonUnity.transform.position = new Vector3(0.0f, -1.0f, 0.0f);

                    dungeonBook.DrawNewDungeon(dungeonUnity.dungeon);

                    SwitchToState(BuildingDungeonNiceState.TURNING_PAGE);
                    break;

                case BuildingDungeonNiceState.TURNING_PAGE:
                    dungeonBook.movingPage.localRotation = Quaternion.Lerp(
                        Quaternion.identity,
                        Quaternion.Euler(0.0f, 0.0f, -180.0f),
                        buildingDungeonNiceTime / ROTATE_PAGE_TIME);

                    if (buildingDungeonNiceTime >= ROTATE_PAGE_TIME)
                        SwitchToState(BuildingDungeonNiceState.EXPANDING_NEW);
                    break;

                case BuildingDungeonNiceState.EXPANDING_NEW:
                    dungeonUnity.gameObject.SetActive(true);
                    dungeonUnity.transform.localScale = new Vector3(1.0f, Mathf.Min(1.0f, buildingDungeonNiceTime / COLLAPSE_TIME), 1.0f);
                    dungeonUnity.transform.position = new Vector3(0.0f, Mathf.Min(0.0f, buildingDungeonNiceTime / COLLAPSE_TIME - 1.0f), 0.0f);
                    if (dungeonUnity.transform.localScale.y == 1.0f)
                        SwitchToState(BuildingDungeonNiceState.MOVING_CAMERA_IN);
                    break;

                case BuildingDungeonNiceState.MOVING_CAMERA_IN:
                    if (buildingDungeonNiceTime > MOVE_CAMERA_TIME)
                    {
                        buildingDungeonNice = false;
                        dungeonBook.movingPage.localRotation = Quaternion.identity;
                        cameraMap.entity = dungeonUnity.avatar;
                        dungeonBook.DrawOldDungeon(dungeonUnity.dungeon);
                    }
                    break;
            }

            buildingDungeonNiceTime += Time.deltaTime;
        }
    }

    private void SwitchToState(BuildingDungeonNiceState newState)
    {
        switch (newState)
        {
            case BuildingDungeonNiceState.MOVING_CAMERA_AWAY:
                cameraFollowEntity.AnimateTo(new Vector3(8, 30, 4), new Vector3(16, 0, 16), MOVE_CAMERA_TIME);
                break;

            case BuildingDungeonNiceState.COLLAPSING_OLD:
                break;

            case BuildingDungeonNiceState.CREATING_NEW:
                break;

            case BuildingDungeonNiceState.TURNING_PAGE:
                break;

            case BuildingDungeonNiceState.EXPANDING_NEW:
                break;

            case BuildingDungeonNiceState.MOVING_CAMERA_IN:
                cameraFollowEntity.AnimateTo(dungeonUnity.avatar, MOVE_CAMERA_TIME);
                break;
        }    

        buildingDungeonNiceState = newState;
        buildingDungeonNiceTime = 0.0f;
    }

}


