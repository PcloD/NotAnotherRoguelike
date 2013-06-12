using UnityEngine;

public class DungeonBook : MonoBehaviour
{
    public Transform movingPage;

    public Renderer newPageRenderer;
    public Renderer oldPageRenderer;

    public Texture2D emptyPageTexture;

    [HideInInspector]
    public Texture2D oldDungeonTexture;

    [HideInInspector]
    public Texture2D newDungeonTexture;

    private Color32[] emptyPagePixels;
    private Color32[] tmpPagePixels;

    public void Awake()
    {
        emptyPagePixels = emptyPageTexture.GetPixels32();
        tmpPagePixels = new Color32[emptyPagePixels.Length];

        oldDungeonTexture = new Texture2D(emptyPageTexture.width, emptyPageTexture.height, TextureFormat.RGB565, true);
        newDungeonTexture = new Texture2D(emptyPageTexture.width, emptyPageTexture.height, TextureFormat.RGB565, true);

        oldDungeonTexture.SetPixels32(emptyPagePixels);
        oldDungeonTexture.Apply();

        newDungeonTexture.SetPixels32(emptyPagePixels);
        newDungeonTexture.Apply();

        newPageRenderer.material.mainTexture = newDungeonTexture;
        oldPageRenderer.material.mainTexture = oldDungeonTexture;
    }

    public void DrawNewDungeon(Dungeon dungeon)
    {
        DrawDungeon(newDungeonTexture, dungeon);
    }

    public void DrawOldDungeon(Dungeon dungeon)
    {
        DrawDungeon(oldDungeonTexture, dungeon);
    }

    private void DrawDungeon(Texture2D texture, Dungeon dungeon)
    {
        System.Array.Copy(emptyPagePixels, tmpPagePixels, emptyPagePixels.Length);

        int tileWidth = texture.width / dungeon.SizeX;
        int tileHeight = texture.height / dungeon.SizeY;

        //Draw surrounding
        DrawSquare(tmpPagePixels, texture.width, texture.height, 0, 0, texture.width, texture.height, new Color32(0, 0, 0, 255));

        for (int x = 0; x < dungeon.SizeX; x++)
        {
            for (int y = 0; y < dungeon.SizeY; y++)
            {
                DungeonTile tile = dungeon.GetTile(x, y);

                int px = x * tileWidth;
                int py = y * tileHeight;

                if (tile.type == DungeonTileType.Wall)
                {
                    DrawSquare(tmpPagePixels, texture.width, texture.height,
                                px + 1, py + 1,
                                tileWidth - 2, tileHeight - 2,
                                new Color32(0, 0, 0, 255));
                }
            }
        }

        texture.SetPixels32(tmpPagePixels);

        texture.Apply(true);
    }

    static private void DrawLine(Color32[] pixels, int textureWidth, int textureHeight, int fromX, int fromY, int toX, int toY, Color32 color)
    {
        if (fromX == toX)
        {
            //Vertical line
            for (int y = fromY; y < toY; y++)
                pixels[fromX + y * textureWidth] = color;
        }
        else if (fromY == toY)
        {
            //Horizontal line
            for (int x = fromX; x < toX; x++)
                pixels[x + fromY * textureWidth] = color;
        }
    }

    static private void DrawSquare(Color32[] pixels, int textureWidth, int textureHeight, int fromX, int fromY, int width, int height, Color32 color)
    {
        DrawLine(pixels, textureWidth, textureHeight, 
                 fromX, fromY, 
                 fromX, fromY + height, 
                 color);

        DrawLine(pixels, textureWidth, textureHeight, 
                 fromX, fromY, 
                 fromX + width, fromY, 
                 color);

        DrawLine(pixels, textureWidth, textureHeight, 
                 fromX, fromY + height - 1, 
                 fromX + width, fromY + height - 1, 
                 color);

        DrawLine(pixels, textureWidth, textureHeight, 
                 fromX + width - 1, fromY,
                 fromX + width - 1, fromY + height, 
                 color);
    }
}



