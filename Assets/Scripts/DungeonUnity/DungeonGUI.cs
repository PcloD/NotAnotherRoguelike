using UnityEngine;

public class DungeonGUI : MonoBehaviour
{
    public DungeonManager dungeonManager;
    
    public void Update()
    {
        if (!dungeonManager.IsBusy())
            UpdatePlayerInput();
    }

    public void OnGUI()
    {
        if (dungeonManager.IsBusy())
            return;

        int size = Mathf.Max(Screen.width, Screen.height) / 10;

        if (GUI.Button(new Rect(10, 10, size, size), "New"))
            dungeonManager.BuildDungeon(true);

        if (GUI.Button(new Rect(10 + size + 10, 10, size, size), "New Nice"))
        {
            dungeonManager.BuildDungeonNiceAnimation();
        }

        if (GUI.Button(new Rect(Screen.width - size - 10, Screen.height - size - 10, size, size), "Camera"))
        {
            if (dungeonManager.cameraFollowEntity.gameObject.activeSelf && !dungeonManager.cameraFollowEntity.firstPerson)
            {
                dungeonManager.cameraFollowEntity.firstPerson = true;

                dungeonManager.dungeonUnity.avatar.model.SetActive(false);
            }
            else if (dungeonManager.cameraFollowEntity.gameObject.activeSelf && dungeonManager.cameraFollowEntity.firstPerson)
            {
                dungeonManager.cameraFollowEntity.gameObject.SetActive(false);
                dungeonManager.cameraMap.gameObject.SetActive(true);

                dungeonManager.dungeonUnity.avatar.model.SetActive(true);
            }
            else
            {
                dungeonManager.cameraFollowEntity.gameObject.SetActive(true);
                dungeonManager.cameraFollowEntity.firstPerson = false;
                dungeonManager.cameraMap.gameObject.SetActive(false);

                dungeonManager.dungeonUnity.avatar.model.SetActive(true);
            }
        }

        if (GUI.Button(new Rect(Screen.width - size - 10, 10, size, size), "Light"))
        {
            if (RenderSettings.ambientLight == Color.white)
                RenderSettings.ambientLight = new Color32(67, 67, 67, 255);
            else
                RenderSettings.ambientLight = Color.white;
        }

        DrawPlayerController();
    }

    public void DrawPlayerController()
    {
        float bottom = Screen.height - 10;
        float left = 10;
        float size = (int) (Mathf.Max(Screen.width, Screen.height) / 10);
        float space = size / 6;

        if (GUI.RepeatButton(new Rect(left, bottom - size, size, size), "Left"))
            MoveAvatar(-1, 0);

        if (GUI.RepeatButton(new Rect(left + (size + space) * 1, bottom - size, size, size), "Down"))
            MoveAvatar(0, -1);

        if (GUI.RepeatButton(new Rect(left + (size + space) * 2, bottom - size, size, size), "Right"))
            MoveAvatar(1, 0);

        if (GUI.RepeatButton(new Rect(left + (size + space) * 1, bottom - (size + space) * 1 - size, size, size), "Up"))
            MoveAvatar(0, 1);
    }

    private void UpdatePlayerInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        MoveAvatar(horizontal, vertical);
    }

    private bool CanMoveAvatar()
    {
        //No input handling while the dungeon is updating
        if (!dungeonManager.dungeonUnity ||
            dungeonManager.dungeonUnity.IsBusy() ||
            !dungeonManager.dungeonUnity.avatar)
        {
            return false;
        }

        return true;
    }

    private void RotateAvatar(float direction)
    {
        if (!CanMoveAvatar())
            return;

        if (direction == 0.0f)
            return;

        dungeonManager.dungeonUnity.UpdateVisibility();

        if (direction > 0)
        {
            int n = (int)dungeonManager.dungeonUnity.avatar.avatar.Rotation;
            n = (n + 1) % 4;
            dungeonManager.dungeonUnity.avatar.avatar.Rotate((DungeonRotation) n);
        }
        else
        {
            int n = (int)dungeonManager.dungeonUnity.avatar.avatar.Rotation;

            n = (n - 1);
            if (n < 0)
                n += 4;

            dungeonManager.dungeonUnity.avatar.avatar.Rotate((DungeonRotation) n);
        }

        dungeonManager.dungeonUnity.UpdateVisibility();
    }

    private void MoveForward()
    {
        if (!CanMoveAvatar())
            return;

        dungeonManager.dungeonUnity.avatar.avatar.Walk(dungeonManager.dungeonUnity.avatar.avatar.Forward);

        dungeonManager.dungeonUnity.UpdateVisibility();
    }

    private void MoveBack()
    {
        if (!CanMoveAvatar())
            return;

        dungeonManager.dungeonUnity.avatar.avatar.Walk(dungeonManager.dungeonUnity.avatar.avatar.Back, false);

        dungeonManager.dungeonUnity.UpdateVisibility();
    }

    private void MoveAvatar(float horizontal, float vertical)
    {
        if (!CanMoveAvatar())
            return;

        if (horizontal == 0 && vertical == 0)
            return;

        bool firstPerson = (dungeonManager.cameraFollowEntity.gameObject.activeSelf && dungeonManager.cameraFollowEntity.firstPerson);

        if (firstPerson)
        {
            if (horizontal > 0)
                RotateAvatar(1.0f);
            else if (horizontal < 0)
                RotateAvatar(-1.0f);

            if (vertical > 0)
                MoveForward();
            else
                MoveBack();
        }
        else
        {
            if (horizontal > 0)
                dungeonManager.dungeonUnity.avatar.avatar.Walk(DungeonVector2.Right);
            else if (horizontal < 0)
                dungeonManager.dungeonUnity.avatar.avatar.Walk(DungeonVector2.Left);
            else if (vertical > 0)
                dungeonManager.dungeonUnity.avatar.avatar.Walk(DungeonVector2.Forward);
            else if (vertical < 0)
                dungeonManager.dungeonUnity.avatar.avatar.Walk(DungeonVector2.Back);

            dungeonManager.dungeonUnity.UpdateVisibility();
        }
    }
}


