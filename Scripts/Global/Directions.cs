using Godot;

public static class Directions
{
    public static readonly Vector2I Up = new(0, -1);
    public static readonly Vector2I TopRight = new(1, -1);
    public static readonly Vector2I Right = new(1, 0);
    public static readonly Vector2I BottomRight = new(1, 1);
    public static readonly Vector2I Down = new(0, 1);
    public static readonly Vector2I BottomLeft = new(-1, 1);
    public static readonly Vector2I Left = new(-1, 0);
    public static readonly Vector2I TopLeft = new(-1, -1);

    public static readonly Vector2I[] directions = {Up, TopRight, Right, BottomRight, Down, BottomLeft, Left, TopLeft};
    public static Vector2I[] GetAllNeighbours(Vector2I position)
    {
        Vector2I[] neighbours = 
        { 
            position + Up,
            position + TopRight,
            position + Right, 
            position + BottomRight, 
            position + Down, 
            position + BottomLeft, 
            position + Left, 
            position + TopLeft
        };   
        return neighbours;
    }

    public static Vector2I[] GetCardinalNeighbours(Vector2I position)
    {
        Vector2I[] neighbours = 
        { 
            position + Up,
            position + Right, 
            position + Down, 
            position + Left, 
        };   
        return neighbours;
    }

}
