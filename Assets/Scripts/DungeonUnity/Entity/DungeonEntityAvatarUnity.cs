using UnityEngine;

public class DungeonEntityAvatarUnity : DungeonEntityUnity
{
    public DungeonEntityAvatar avatar;

    public override void Init(DungeonUnity dungeonUnity, DungeonEntity entity)
    {
        base.Init(dungeonUnity, entity);

        avatar = (DungeonEntityAvatar)entity;
    }
}


