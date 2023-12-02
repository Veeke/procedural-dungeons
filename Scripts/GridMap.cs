using System;
using Godot;

public partial class GridMap
{
    public Tile[,] grid;

    public GridMap(Tile[,] grid)
    {
        this.grid = grid;
    }
    
    public Vector2I GetGridSize()
    {
        Vector2I gridSize = new (grid.GetLength(0), grid.GetLength(1));
        return gridSize;
    }

    public Tile GetCell(Vector2I position) => grid[position.X, position.Y];

    public Tile GetCell(int x, int y) => grid[x, y];

    public Tile[] GetCells(Vector2I[] positions)
    {
        Tile[] tiles = new Tile[positions.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            if (GridManager.IsValidCoord(positions[i]))
            {
                tiles[i] = GetCell(positions[i]);
            }
            else
            {
                tiles[i] = null;
            }
        }
        return tiles;
    }

    public void InitializeCell(int x, int y)
    {
        Tile newTile = new();
        grid[x, y] = newTile;
    }
}
