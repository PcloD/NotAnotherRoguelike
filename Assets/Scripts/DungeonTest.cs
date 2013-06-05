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
        IDungeonGenerator generator = new DungeonGeneratorEmpty();

        Dungeon dungeon = generator.BuildDungeon(sizeX, sizeY);

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


