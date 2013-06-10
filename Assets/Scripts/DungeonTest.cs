using UnityEngine;

public class DungeonTest : MonoBehaviour
{
    public int sizeX = 64;
    public int sizeY = 64;
    public DungeonUnity dungeonUnity;

    public void Start()
    {
        RenderSettings.ambientLight = new Color32(67, 67, 67, 255);

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
            BuildDungeon();

        if (GUI.Button(new Rect(Screen.width - size - 10, 10, size, size), "Light"))
        {
            if (RenderSettings.ambientLight == Color.white)
                RenderSettings.ambientLight = new Color32(67, 67, 67, 255);
            else
                RenderSettings.ambientLight = Color.white;

        }
    }
}


