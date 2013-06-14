using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonMapUnity), true)]
public class DungeonMapUnityCustomInspector : Editor
{
    private DungeonMapUnity dungeonMap;
    private bool sendMove;

    public void OnSceneGUI()
    {
        dungeonMap = (DungeonMapUnity)target;

        if (dungeonMap.IsEditing())
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            switch(Event.current.type)
            {
                case EventType.MouseDown:
                    if (!Event.current.alt && !Event.current.shift && !Event.current.control)
                    {
                        dungeonMap.EditorMouseClickOrMove(Event.current);
                        sendMove = true;
                        EditorUtility.SetDirty(dungeonMap);
                    }
                    break;

                case EventType.MouseUp:
                    sendMove = false;
                    break;

                case EventType.MouseMove:
                case EventType.MouseDrag:
                    if (sendMove)
                        dungeonMap.EditorMouseClickOrMove(Event.current);
                    break;

                case EventType.Layout:
                    HandleUtility.AddDefaultControl(controlId);
                    break;
            }
        }

        DrawEditor();
    }

    private void DrawEditor()
    {
        Handles.BeginGUI();

        if (!dungeonMap.IsEditing())
        {
            if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 80, 80, 30), "Edit Map"))
                dungeonMap.StartEdit();
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 80 - 50 * 1, 80, 30), "Wall"))
                DungeonMapUnity.paintingTileType = DungeonTileType.Wall;

            if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 80 - 50 * 2, 80, 30), "Empty"))
                DungeonMapUnity.paintingTileType = DungeonTileType.Empty;

            if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 80 - 50 * 3, 80, 30), "Reset"))
            {
                dungeonMap.tiles = new DungeonTileType[dungeonMap.sizeX * dungeonMap.sizeY];
                dungeonMap.UpdateEditorMesh();
            }

            if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 80, 80, 30), "Exit Edit"))
                dungeonMap.EndEdit();
        }

        Handles.EndGUI();
    }
}


