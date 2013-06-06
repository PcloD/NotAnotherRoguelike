using UnityEngine;

public class DungeonTest : MonoBehaviour
{
    public int sizeX = 64;
    public int sizeY = 64;
    public DungeonUnity dungeonUnity;

    public void Start()
    {
        BuildDungeon();
    }

    public void BuildDungeon()
    {
        DungeonBuilder builder = new DungeonBuilder();

        builder.BeginDungeon(sizeX, sizeY);

        builder.Decorate();

        builder.AddEntities();

        Dungeon dungeon = builder.GetDungeon();

        dungeonUnity.SetDungeon(dungeon);
    }

    public void OnGUI()
    {
        int size = Mathf.Max(Screen.width, Screen.height) / 10;

        if (GUI.Button(new Rect(10, 10, size * 2, size), "Rebuild Dungeon"))
        {
            BuildDungeon();
        }
    }
}


