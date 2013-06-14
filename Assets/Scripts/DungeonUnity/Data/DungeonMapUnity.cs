using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class DungeonMapUnity : MonoBehaviour 
{
    public int sizeX;
    public int sizeY;
    public DungeonTileType[] tiles;

#if UNITY_EDITOR
    private int[] triangleToTileMapping;
    static public DungeonTileType paintingTileType;

    public bool IsEditing()
    {
        return GetComponent<MeshFilter>() && GetComponent<MeshRenderer>() && GetComponent<MeshCollider>();
    }

    public void StartEdit()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshRenderer.castShadows = false;
            meshRenderer.receiveShadows = false;
            meshRenderer.sharedMaterial = new Material(Shader.Find("Custom/VertexColored"));
        }

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (!meshCollider)
            meshCollider = gameObject.AddComponent<MeshCollider>();

        UpdateEditorMesh();
    }

    public void EndEdit()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
            if (meshFilter.sharedMesh)
                Component.DestroyImmediate(meshFilter.sharedMesh);

            GameObject.DestroyImmediate(meshFilter);
        }

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer)
        {
            if (meshRenderer.sharedMaterial)
                GameObject.DestroyImmediate(meshRenderer.sharedMaterial);

            GameObject.DestroyImmediate(meshRenderer);
        }

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider)
            GameObject.DestroyImmediate(meshCollider);
    }

    public void Update()
    {
        if (IsEditing())
            UpdateDungeonSize();
    }

    private void UpdateDungeonSize()
    {
        if (tiles.Length != sizeX * sizeY)
            tiles = new DungeonTileType[sizeX * sizeY];
    }

    public void UpdateEditorMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        if (!meshFilter || !meshCollider)
            return;

        UpdateDungeonSize();

        Mesh mesh = meshFilter.sharedMesh;
        if (!mesh)
            mesh = new Mesh();

        Vector3[] vertices = mesh.vertices;
        Color32[] colors = mesh.colors32;
        int[] triangles = mesh.triangles;

        int numVertices = sizeX * sizeY * 4;
        int numTriangles = sizeX * sizeY * 6;

        if (vertices == null || vertices.Length != numVertices)
            vertices = new Vector3[numVertices];

        if (colors == null || colors.Length != numVertices)
            colors = new Color32[numVertices];

        if (triangles == null || triangles.Length != numTriangles)
            triangles = new int[numTriangles];

        if (triangleToTileMapping == null || triangleToTileMapping.Length != sizeX * sizeY * 2)
            triangleToTileMapping = new int[sizeX * sizeY * 2];

        int vIndex = 0;
        int tIndex = 0;
        int tIndexMapping = 0;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                DungeonTileType tileType = tiles[x + y * sizeX];
                Color32 color;

                switch (tileType)
                {
                    case DungeonTileType.Wall:
                        color = Color.red;
                        break;

                    default:
                        color = Color.white;
                        break;
                }

                vertices[vIndex + 0] = new Vector3(x, 0, y);
                vertices[vIndex + 1] = new Vector3(x + 1, 0, y);
                vertices[vIndex + 2] = new Vector3(x + 1, 0, y + 1);
                vertices[vIndex + 3] = new Vector3(x, 0, y + 1);

                colors[vIndex + 0] = color;
                colors[vIndex + 1] = color;
                colors[vIndex + 2] = color;
                colors[vIndex + 3] = color;

                triangles[tIndex + 0] = vIndex + 2;
                triangles[tIndex + 1] = vIndex + 1;
                triangles[tIndex + 2] = vIndex + 0;

                triangles[tIndex + 3] = vIndex + 0;
                triangles[tIndex + 4] = vIndex + 3;
                triangles[tIndex + 5] = vIndex + 2;

                triangleToTileMapping[tIndexMapping + 0] = (x << 16) | y;
                triangleToTileMapping[tIndexMapping + 1] = (x << 16) | y;

                vIndex += 4;
                tIndex += 6;
                tIndexMapping += 2;
            }
        }

        mesh.vertices = vertices;
        mesh.colors32 = colors;
        mesh.triangles = triangles;

        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void EditorMouseClickOrMove(Event currentEvent)
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (!meshCollider)
            return;

        if (triangleToTileMapping == null)
            return;

        if (currentEvent.button == 0)
        {
            RaycastHit hit;

            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);

            if (meshCollider.Raycast(ray, out hit, 1000.0f))
            {
                if (hit.triangleIndex >= triangleToTileMapping.Length)
                    return;

                currentEvent.Use();

                int codedIndex = triangleToTileMapping[hit.triangleIndex];

                int x = (codedIndex >> 16);
                int y = (codedIndex & 0xFFFF);

                tiles[x + y * sizeX] = paintingTileType;

                UpdateEditorMesh();
            }
        }
    }

#endif
}
