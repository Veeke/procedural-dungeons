using Godot;
using System;
using System.Collections.Generic;

public partial class GridManager: Node
{
    public static GridMap map;
    private static TileMap tilemap;
    public static List<Vector2I> entrances;
    public static Vector2I PlayerPosition {get; set;}

    public override void _Ready()
    {
        tilemap = GetNodeOrNull<TileMap>("../Floor Generator/TileMap");
    }

    public static void InitializeGrid(Vector2I gridSize, IEnumerable<Vector2I> positions)
    {
        Tile[,] grid = new Tile[gridSize.X, gridSize.Y];
        map = new GridMap(grid);

        for (int y = 0; y < gridSize.Y; y++)
        {
            for (int x = 0; x < gridSize.X; x++)
            {
                map.InitializeCell(x, y);
                map.GetCell(x, y).TerrainType = Tile.Terrain.Wall;
                map.GetCell(x, y).coordinates = new Vector2I(x, y);
            }
        }
        foreach (Vector2I position in positions)
        {
            if (!IsValidCoord(position))
            {
                GD.Print("Invalid position given in InitializeGrid() at: " + position);
            }
            map.GetCell(position).TerrainType = Tile.Terrain.Floor;
        }
    }

    public static void SetStairLocation(Vector2I position)
    {
        map.GetCell(position).ContentType = Tile.Content.Stairs;
    }

    public static bool IsValidCoord(int x, int y)
    {
        if (x > 0 && x < map.grid.GetLength(0) && y > 0 && y < map.grid.GetLength(1))
        {
            return true;
        }
        else return false;
    }

    public static bool IsValidCoord(Vector2I position)
    {
        if (position.X > 0 && position.X < map.grid.GetLength(0) && position.Y > 0 && position.Y < map.grid.GetLength(1))
        {
            return true;
        }
        else return false;
    }

    public static bool BlocksLight(int x, int y)
    {
        if (IsValidCoord(x, y))
        {
            return map.GetCell(x, y).IsWall();
        }
        else return true;
    }

    public static void SetVisible(int x, int y, bool visibility)
    {
        if (IsValidCoord(x, y))
        {
            map.GetCell(x, y).IsVisible = visibility;
        }
    }

    public static void SetExplored(int x, int y, bool isExplored)
    {
        if (IsValidCoord(x, y))
        {
            map.GetCell(x, y).IsExplored = isExplored;
        }
    }

    public static Vector2I ConvertGridToWorld(Vector2I position)
    {
        return (Vector2I)tilemap.MapToLocal(position);
    }

    public static void ResetVision(Vector2I origin, int visionRange)
    {
        for (int y = origin.Y - visionRange - 2; y <= origin.Y + visionRange + 2; y++)
        {
            for (int x = origin.X - visionRange - 2; x <= origin.X + visionRange + 2; x++)
            {
                SetVisible(x, y, false);
            }
        }
    }
}
