using System.Reflection.Metadata.Ecma335;
using Godot;

public partial class Tile
{
    public enum Terrain {Floor, Wall};
    public enum Content {Empty, Stairs, Item};
    public Terrain TerrainType {get; set;}
    public Content ContentType {get; set;}
    public bool IsVisible {get; set;}
    public bool IsExplored {get; set;}
    public bool IsOccupied {get; set;}

    // For A* pathfinding
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public Tile parent;

    public Vector2I coordinates;

    public bool IsFloor()
    {
        return TerrainType == Terrain.Floor;
    } 
    public bool IsWall()
    {
        return TerrainType == Terrain.Wall;
    } 
    public bool IsWalkable()
    {
        return !IsOccupied && TerrainType == Terrain.Floor;
    }
    public bool IsStairs() => ContentType == Content.Stairs;
}
