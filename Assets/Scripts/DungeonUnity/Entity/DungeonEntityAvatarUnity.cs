using UnityEngine;

public class DungeonEntityAvatarUnity : DungeonEntityUnity
{
    public DungeonEntityAvatar avatar;

    public override void Init(DungeonUnity dungeonUnity, DungeonEntity entity)
    {
        base.Init(dungeonUnity, entity);

        avatar = (DungeonEntityAvatar)entity;
    }

    public void Update()
    {
        UpdatePlayerInput();
    }

    public void OnGUI()
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

    private void MoveAvatar(float horizontal, float vertical)
    {
        //No input handling while the dungeon is updating
        if (dungeonUnity.IsBusy())
            return;

        if (horizontal == 0 && vertical == 0)
            return;

        if (horizontal > 0)
            avatar.Walk(DungeonVector2.Right);
        else if (horizontal < 0)
            avatar.Walk(DungeonVector2.Left);
        else if (vertical > 0)
            avatar.Walk(DungeonVector2.Forward);
        else if (vertical < 0)
            avatar.Walk(DungeonVector2.Back);

        dungeonUnity.UpdateVisibility();
    }

}


