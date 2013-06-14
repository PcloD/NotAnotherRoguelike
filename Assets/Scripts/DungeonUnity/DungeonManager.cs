using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private const float MOVE_CAMERA_TIME = 1.0f;
    private const float COLLAPSE_TIME = 1.0f;
    private const float ROTATE_PAGE_TIME = 1.5f;

    private enum BuildingDungeonNiceState
    {
        MOVING_CAMERA_AWAY,
        COLLAPSING_OLD,
        MOVING_CAMERA_AWAY_AND_COLLAPSING_OLD,
        CREATING_NEW_1,
        CREATING_NEW_2,
        CREATING_NEW_3,
        CREATING_NEW_4,
        TURNING_PAGE,
        EXPANDING_NEW,
        MOVING_CAMERA_IN,
        MOVING_CAMERA_IN_AND_EXPANDING_NEW
    }

    public bool moveAndZoomWhileBuilding = true;

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
    private Vector3 buildingDungeonNiceNewAvatarPosition;

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

        DungeonMap dungeon = builder.GetDungeon();

        SetDungeon(dungeon, updateCamera);
    }

    public void SetDungeon(DungeonMap dungeon, bool updateCamera = true)
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

    public void BuildDungeonNiceAnimation()
    {
        buildingDungeonNice = true;

        cameraFollowEntity.gameObject.SetActive(true);
        cameraFollowEntity.firstPerson = false;
        cameraMap.gameObject.SetActive(false);

        cameraFollowEntity.entity = null;

        if (moveAndZoomWhileBuilding)
            SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.MOVING_CAMERA_AWAY_AND_COLLAPSING_OLD);
        else
            SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.MOVING_CAMERA_AWAY);
    }

    public void Update()
    {
        if (buildingDungeonNice)
        {
            switch (buildingDungeonNiceState)
            {
                case BuildingDungeonNiceState.MOVING_CAMERA_AWAY:
                    if (!cameraFollowEntity.IsBusy())
                        SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.COLLAPSING_OLD);
                    break;

                case BuildingDungeonNiceState.COLLAPSING_OLD:
                    dungeonUnity.transform.localScale = new Vector3(1.0f, Mathf.Max(0.0f, 1.0f - buildingDungeonNiceTime / COLLAPSE_TIME), 1.0f);
                    dungeonUnity.transform.position = new Vector3(0.0f, -(buildingDungeonNiceTime / COLLAPSE_TIME) * 0.5f, 0.0f);
                    if (dungeonUnity.transform.localScale.y == 0.0f)
                    {
                        dungeonUnity.gameObject.SetActive(false);
                        SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.CREATING_NEW_1);
                    }
                    break;

                case BuildingDungeonNiceState.MOVING_CAMERA_AWAY_AND_COLLAPSING_OLD:
                    dungeonUnity.transform.localScale = new Vector3(1.0f, Mathf.Max(0.0f, 1.0f - buildingDungeonNiceTime / COLLAPSE_TIME), 1.0f);
                    dungeonUnity.transform.position = new Vector3(0.0f, -(buildingDungeonNiceTime / COLLAPSE_TIME) * 0.5f, 0.0f);
                    if (dungeonUnity.transform.localScale.y == 0.0f && !cameraFollowEntity.IsBusy())
                    {
                        dungeonUnity.gameObject.SetActive(false);
                        SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.CREATING_NEW_1);
                    }
                    break;

                case BuildingDungeonNiceState.CREATING_NEW_1:
                    //Wait 1 frame
                    SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.CREATING_NEW_2);
                    break;

                case BuildingDungeonNiceState.CREATING_NEW_2:
                    dungeonUnity.gameObject.SetActive(true);
                    dungeonUnity.transform.localScale = Vector3.one;
                    dungeonUnity.transform.position = Vector3.zero;

                    BuildDungeon(false);

                    buildingDungeonNiceNewAvatarPosition = dungeonUnity.avatar.trans.position;

                    dungeonUnity.gameObject.SetActive(false);
                    dungeonUnity.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
                    dungeonUnity.transform.position = new Vector3(0.0f, -0.5f, 0.0f);

                    SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.CREATING_NEW_3);
                    break;

                case BuildingDungeonNiceState.CREATING_NEW_3:
                    dungeonBook.DrawNewDungeon(dungeonUnity.dungeon);
                    SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.CREATING_NEW_4);
                    break;

                case BuildingDungeonNiceState.CREATING_NEW_4:
                    //Wait 1 frame
                    SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.TURNING_PAGE);
                    break;

                case BuildingDungeonNiceState.TURNING_PAGE:
                    dungeonBook.movingPage.localRotation = Quaternion.Lerp(
                        Quaternion.identity,
                        Quaternion.Euler(0.0f, 0.0f, -180.0f),
                        buildingDungeonNiceTime / ROTATE_PAGE_TIME);

                    if (buildingDungeonNiceTime >= ROTATE_PAGE_TIME)
                    {
                        if (moveAndZoomWhileBuilding)
                            SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.MOVING_CAMERA_IN_AND_EXPANDING_NEW);
                        else
                            SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.EXPANDING_NEW);
                    }
                    break;

                case BuildingDungeonNiceState.EXPANDING_NEW:
                    dungeonUnity.gameObject.SetActive(true);
                    dungeonUnity.transform.localScale = new Vector3(1.0f, Mathf.Min(1.0f, buildingDungeonNiceTime / COLLAPSE_TIME), 1.0f);
                    dungeonUnity.transform.position = new Vector3(0.0f, Mathf.Min(0.0f, (buildingDungeonNiceTime / COLLAPSE_TIME) * 0.5f - 0.5f), 0.0f);
                    if (dungeonUnity.transform.localScale.y == 1.0f)
                        SwitchBuildingDungeonNiceState(BuildingDungeonNiceState.MOVING_CAMERA_IN);
                    break;

                case BuildingDungeonNiceState.MOVING_CAMERA_IN:
                    if (!cameraFollowEntity.IsBusy())
                    {
                        buildingDungeonNice = false;
                        dungeonBook.movingPage.localRotation = Quaternion.identity;
                        cameraFollowEntity.entity = dungeonUnity.avatar;
                        cameraMap.entity = dungeonUnity.avatar;
                        dungeonBook.DrawOldDungeon(dungeonUnity.dungeon);
                    }
                    break;

                case BuildingDungeonNiceState.MOVING_CAMERA_IN_AND_EXPANDING_NEW:
                    dungeonUnity.gameObject.SetActive(true);
                    dungeonUnity.transform.localScale = new Vector3(1.0f, Mathf.Min(1.0f, buildingDungeonNiceTime / COLLAPSE_TIME), 1.0f);
                    dungeonUnity.transform.position = new Vector3(0.0f, Mathf.Min(0.0f, (buildingDungeonNiceTime / COLLAPSE_TIME) * 0.5f - 0.5f), 0.0f);
                    if (dungeonUnity.transform.localScale.y == 1.0f && !cameraFollowEntity.IsBusy())
                    {
                        buildingDungeonNice = false;
                        dungeonBook.movingPage.localRotation = Quaternion.identity;
                        cameraFollowEntity.entity = dungeonUnity.avatar;
                        cameraMap.entity = dungeonUnity.avatar;
                        dungeonBook.DrawOldDungeon(dungeonUnity.dungeon);
                    }
                    break;
            }

            buildingDungeonNiceTime += Time.deltaTime;
        }
    }

    private void SwitchBuildingDungeonNiceState(BuildingDungeonNiceState newState)
    {
        switch (newState)
        {
            case BuildingDungeonNiceState.MOVING_CAMERA_AWAY:
            case BuildingDungeonNiceState.MOVING_CAMERA_AWAY_AND_COLLAPSING_OLD:
                cameraFollowEntity.AnimateTo(new Vector3(16, 32, -8), new Vector3(16, 0, 16), MOVE_CAMERA_TIME);
                break;

            case BuildingDungeonNiceState.MOVING_CAMERA_IN:
            case BuildingDungeonNiceState.MOVING_CAMERA_IN_AND_EXPANDING_NEW:
                cameraFollowEntity.AnimateTo(buildingDungeonNiceNewAvatarPosition, MOVE_CAMERA_TIME);
                break;
        }    

        buildingDungeonNiceState = newState;
        buildingDungeonNiceTime = 0.0f;
    }

    public bool IsBusy()
    {
        return buildingDungeonNice;
    }

}


