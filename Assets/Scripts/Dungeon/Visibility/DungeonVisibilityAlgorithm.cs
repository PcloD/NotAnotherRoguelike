using System;
using System.Collections.Generic;

public class DungeonVisibilityAlgorithm
{
    public void Reset(Dungeon dungeon)
    {
        for (int x = 0; x < dungeon.SizeX; x++)
            for (int y = 0; y < dungeon.SizeY; y++)
                dungeon.SetTileVisible(x, y, false);
    }

    public void SetVisible(Dungeon dungeon, int lightX, int lightY, int lightRadius)
    {
        for (int x = 0; x < dungeon.SizeX; x++)
        {
            for (int y = 0; y < dungeon.SizeY; y++)
            {
                int dx = x - lightX;
                int dy = y - lightY;

                if (Math.Abs(dx) + Math.Abs(dy) <= lightRadius)
                {
                    if (IsVisible(dungeon, lightX, lightY, x, y, 50, 50) ||
                        IsVisible(dungeon, lightX, lightY, x, y, 0, 0) ||
                        IsVisible(dungeon, lightX, lightY, x, y, 0, 99) ||
                        IsVisible(dungeon, lightX, lightY, x, y, 99, 99) ||
                        IsVisible(dungeon, lightX, lightY, x, y, 99, 0))
                    {
                        dungeon.SetTileVisible(x, y, true);
                    }
                }
            }
        }
    }

    public bool IsVisible(Dungeon dungeon, int fromX, int fromY, int toX, int toY, int offsetX = 50, int offsetY = 50)
    {
        int steps;
        int deltaX = toX - fromX;
        int deltaY = toY - fromY;
        int initX;
        int initY;

        if (Math.Abs(deltaX) > Math.Abs(deltaY))
            steps = Math.Abs(deltaX);
        else
            steps = Math.Abs(deltaY);

        if (steps == 0)
            return(true);

        initX = fromX * 100 + offsetX;
        initY = fromY * 100 + offsetY;

        deltaY *= 100;
        deltaX *= 100;

        bool visible = true;

        for (int i = 0; i <= steps; i++)
        {
            int tileX = (deltaX * i / steps + initX) / 100;
            int tileY = (deltaY * i / steps + initY) / 100;

            if (visible)
            {
                if (dungeon.GetTile(tileX, tileY).solid)
                    visible = false;
            }
            else
            {
                return(false);
            }
        }

        return(true);
    }
}


