using Godot;
using System.Collections.Generic;

public partial class PathfindingComponent : Node
{
    [Export]
    private MoveComponent moveComponent;
    [Export]
    private Actor actor;
    private GameEvents gameEvents;

    private static List<Vector2I> path = new();
    private bool SkipTurn = true;
    private Vector2I currentPosition;

    [Export]
    private PathfindDebug debug;

    public override void _Ready()
    {
        gameEvents = GetNode<GameEvents>("/root/GameEvents");
        gameEvents.Connect(GameEvents.SignalName.PlayerMoved, new Callable(this, MethodName.MoveInPath));
        gameEvents.Connect(GameEvents.SignalName.StairsReached, new Callable(this, MethodName.ResetPathfinding));
    }

    public void MoveInPath(Vector2I position)
    {
        Vector2I direction;

        // If the enemy can "see" the player (FOV is symmetric), the player becomes the target
        if (GridManager.map.GetCell(actor.GridPosition).IsVisible)
        {
            FindPath(actor.GridPosition, GridManager.PlayerPosition);
            debug.SetPlayerPos(GridManager.PlayerPosition);
        }

        // If the path length is shorter than two it's either empty or we've already reached the target, so create a new path
        else if (path.Count < 2)
        {
            debug.SetPlayerPos(Vector2I.Zero);
            Vector2I target = actor.GridPosition;
            while (target == actor.GridPosition)
            {
                int randomIndex = Random.rng.RandiRange(0, GridManager.entrances.Count - 1);
                target = GridManager.entrances[randomIndex];
            }
            FindPath(actor.GridPosition, target);
        }

        // SkipTurn check is to prevent the enemy from moving on the floor's initialization
        if (!SkipTurn)
        {
            direction = path[1] - path[0];
            moveComponent.MoveOnGrid(direction);
            if (actor.GridPosition == path[1])
            {
                path.RemoveAt(0);
            }
            debug.SetPath(path);
        }
        else SkipTurn = false;    
    }

    public void FindPath(Vector2I startPosition, Vector2I targetPosition)
    {
        List<Tile> openSet = new();
        HashSet<Tile> closedSet = new();
        Tile startTile = GridManager.map.GetCell(startPosition);
        Tile targetTile = GridManager.map.GetCell(targetPosition);
        openSet.Add(startTile);

        while (openSet.Count > 0)
        {
            Tile currentTile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentTile.fCost || openSet[i].fCost == currentTile.fCost && openSet[i].hCost < currentTile.hCost)
                {
                    currentTile = openSet[i];
                }
            }
            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if (currentTile == targetTile)
            {
                RetracePath(startTile, targetTile);
                return;
            }

            Tile[] neighbours = GridManager.map.GetCells(Directions.GetAllNeighbours(currentTile.coordinates));
            for(int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] == null || !neighbours[i].IsFloor() || closedSet.Contains(neighbours[i]))
                {
                    continue;
                }
                // Prevent cutting corners
                if (i % 2 == 1 && (!neighbours[i + 1 == neighbours.Length ? 0 : i + 1].IsFloor() || !neighbours[i-1].IsFloor()))
                {
                    continue;
                }

                int newMoveCostToNeighbour = currentTile.gCost + GetDistance(currentTile, neighbours[i]);
                if (newMoveCostToNeighbour < neighbours[i].gCost || !openSet.Contains(neighbours[i]))
                {
                    neighbours[i].gCost = newMoveCostToNeighbour;
                    neighbours[i].hCost = GetDistance(neighbours[i], targetTile);
                    neighbours[i].parent = currentTile;

                    if (!openSet.Contains(neighbours[i]))
                    {
                        openSet.Add(neighbours[i]);
                    }
                }
            }
        }
    }

    public void RetracePath (Tile start, Tile end)
    {
        List<Vector2I> newPath = new();
        Tile current = end;

        while (current != start)
        {
            newPath.Add(current.coordinates);
            current = current.parent;
        }
        newPath.Add(actor.GridPosition);
        newPath.Reverse();
        path = newPath;
    }

    public void ResetPathfinding()
    {
        path.Clear();
        SkipTurn = true;
        debug.SetPlayerPos(Vector2I.Zero);
    }

    static int GetDistance(Tile pointA, Tile pointB)
    {
        int x = Mathf.Abs(pointA.coordinates.X - pointB.coordinates.X);
        int y = Mathf.Abs(pointA.coordinates.Y - pointB.coordinates.Y);

        if (x > y)
        {
            return 14 * y + 10 * (x - y);
        }
        return 14 * x + 10 * (y - x);
    }
}
