using UnityEngine;

public class DungeonTest : MonoBehaviour
{
    public int sizeX = 64;
    public int sizeY = 64;
    public DungeonRendererUnity dungeonRenderer;

    public void Start()
    {
        BuildDungeon();
    }

    public void BuildDungeon()
    {
        DungeonBuilder builder = new DungeonBuilder();

        builder.BeginDungeon(sizeX, sizeY);

        builder.AddEntities();

        Dungeon dungeon = builder.GetDungeon();

        dungeonRenderer.DrawDungeon(dungeon);
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Rebuild Dungeon"))
        {
            BuildDungeon();
        }
    }
}


