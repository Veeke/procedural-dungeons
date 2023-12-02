using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class FloorGenerator : Node
{
    [Export(PropertyHint.Range, "1, 10,")]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [Export(PropertyHint.Range, "1, 10,")]
    private int maxRoomWidth = 8, maxRoomHeight = 8;
    [Export(PropertyHint.Range, "24, 64,")]
    private int floorWidth = 32, floorHeight = 32;
    
    [Export(PropertyHint.Range, "1, 8,")]
    private int horizontalChunks = 2, verticalChunks = 2;
    [Export(PropertyHint.Range, "1, 3,")]
    private int minDistance = 1;
    [Export]
    private int dummyRoomCount = 1;
    
    //Vector2I containers of the above properties
    private Vector2I minRoomSize;
    private Vector2I maxRoomSize;
    private Vector2I floorSize;

    [Export]
    private TilemapVisualizer tilemapVisualizer;
    [Export]
    private MinimapVisualizer minimapVisualizer;
    [Export]
    private Actor player;
    [Export]
    private Actor enemy;
    private HashSet<Vector2I> entrances;

    // Autoload Scripts
    private GameEvents gameEvents;

    public override void _Ready()
    { 
        Random.rng.Randomize();

        minRoomSize = new Vector2I (minRoomWidth, minRoomHeight);
        maxRoomSize = new Vector2I (maxRoomWidth, maxRoomHeight);
        floorSize = new Vector2I (floorWidth, floorHeight);

        dummyRoomCount = Mathf.Min(dummyRoomCount, horizontalChunks * verticalChunks - 1);

        gameEvents = GetNode<GameEvents>("/root/GameEvents");
        gameEvents.Connect(GameEvents.SignalName.StairsReached, new Callable(this, MethodName.ResetFloor));

        entrances = new HashSet<Vector2I>();
    }

    private void CreateFloor()
    {
        entrances.Clear();
        List<Rect2I> roomList = ProceduralGenerationAlgorithms.BinarySpacePartioning(
            floorSize, horizontalChunks, verticalChunks, 
            minRoomSize, maxRoomSize, minDistance);

        HashSet<Vector2I> rooms = CreateSimpleRooms(roomList);
        HashSet<Vector2I> corridors = CreateCorridors(roomList);
        HashSet<Vector2I> floor = new();

        floor.UnionWith(rooms);
        floor.UnionWith(corridors);
        InitializeFloor(floor);
        
        // Remove room entrances & dummy rooms as valid positions to initialize actors on
        GridManager.entrances = entrances.ToList(); 
        foreach(Vector2I entrance in entrances)
        {
            RemovePoint(rooms, entrance);
        }
        InitializeStairs(rooms);
        InitializeActors(rooms);
    }

    public void InitializeFloor(HashSet<Vector2I> floor)
    {
        GridManager.InitializeGrid(floorSize, floor);
        tilemapVisualizer.VisualizeMap(GridManager.map);     
    }

    public void ResetFloor()
    {
        tilemapVisualizer.ResetTilemap();
        minimapVisualizer.ResetTilemap();
        CreateFloor();
    }

    private HashSet<Vector2I> CreateSimpleRooms(List<Rect2I> roomList)
    {
        HashSet<Vector2I> rooms = new();

        //Replace some rooms by 1x1 dummy rooms for variety & extra long corridors
        for (int i = 0; i < dummyRoomCount; i++)
        {
            int randomIndex = Random.rng.RandiRange(0, roomList.Count - 1);
            roomList[randomIndex] = new Rect2I(roomList[randomIndex].Position, Vector2I.One);
            entrances.Add(roomList[randomIndex].Position);
        }

        //Convert the rooms into a collection of points on the grid
        foreach (Rect2I room in roomList)
        {
            for (int col = 0; col < room.Size.X; col++)
            {
                for (int row = 0; row < room.Size.Y; row++)
                {
                    Vector2I position = room.Position + new Vector2I(col, row);
                    rooms.Add(position);
                }
            }
        }
        return rooms;
    }

    //Creates corridors between adjacent rooms
    private HashSet<Vector2I> CreateCorridors(List<Rect2I> roomList)
    {
        HashSet<Vector2I> corridors = new();

        //Start is a random point on an outer edge of the current room
        //End is a random point on an outer edge of an adjacent room
        //The collinear points are chosen to lie somewhere between start and end, and the line through them is perpendicular to the connected edges.
        Vector2I startPoint;
        Vector2I endPoint;
        Vector2I collinearPoint1;
        Vector2I collinearPoint2;

        for (int i = 0; i < roomList.Count - 1; i++)
        {
            Rect2I startRoom = roomList[i];
            Rect2I endRoom;

            //Sideways corridor setup
            //Sideways corridors are added to any room that is not in the rightmost column of chunks
            if ((i + 1) % horizontalChunks != 0)
            {
                endRoom = roomList[i + 1];

                startPoint = new Vector2I(startRoom.End.X - 1, Random.rng.RandiRange(startRoom.Position.Y, startRoom.End.Y - 1));
                endPoint = new Vector2I(endRoom.Position.X, Random.rng.RandiRange(endRoom.Position.Y, endRoom.End.Y - 1));
                collinearPoint1 = new Vector2I(Random.rng.RandiRange(startPoint.X + 2, endPoint.X - 2), startPoint.Y);
                collinearPoint2 = new Vector2I(collinearPoint1.X, endPoint.Y);

                Vector2I[] points = { startPoint, collinearPoint1, collinearPoint2, endPoint };

                corridors.UnionWith(ConnectPoints(points));
                entrances.Add(startPoint);
                entrances.Add(endPoint);
            }

            //Downward corridor setup
            //Downward corridors are only added if they're not on the final row of chunks
            //They are guaranteed if the room is in one of the outer columns of chunks, otherwise the chance is 50%
            if (i < roomList.Count - horizontalChunks)
            {
                if ((i + 1) % horizontalChunks == 0 || (i + 1) % horizontalChunks == 1 || Random.rng.Randf() > 0.5)
                {
                    endRoom = roomList[i + horizontalChunks];
                
                    startPoint = new Vector2I(Random.rng.RandiRange(startRoom.Position.X, startRoom.End.X - 1), startRoom.End.Y - 1);          
                    endPoint = new Vector2I(Random.rng.RandiRange(endRoom.Position.X, endRoom.End.X - 1), endRoom.Position.Y);    
                    collinearPoint1 = new Vector2I(startPoint.X, Random.rng.RandiRange(startPoint.Y + 2, endPoint.Y - 2));           
                    collinearPoint2 = new Vector2I(endPoint.X, collinearPoint1.Y);

                    Vector2I[] points = { startPoint, collinearPoint1, collinearPoint2, endPoint };

                    corridors.UnionWith(ConnectPoints(points));
                    entrances.Add(startPoint);
                    entrances.Add(endPoint);
                }      
            }                      
        }         
        return corridors;
    }

    private static HashSet<Vector2I> ConnectPoints(Vector2I[] points)
    {
        HashSet<Vector2I> path = new();
        Vector2I currentPos = points[0];

        for (int i = 1; i < points.Length; i++)
        {
            while (currentPos.X != points[i].X)
            {
                if (currentPos.X < points[i].X)
                {
                    currentPos += Vector2I.Right;
                }
                else if (currentPos.X > points[i].X)
                {
                    currentPos += Vector2I.Left;
                }
                path.Add(currentPos);
            }
            while (currentPos.Y != points[i].Y)
            {
                if (currentPos.Y < points[i].Y)
                {
                    currentPos += Vector2I.Down;
                }
                else if (currentPos.Y > points[i].Y)
                {
                    currentPos += Vector2I.Up;
                }
                path.Add(currentPos);
            }
        }
        return path;
    }

    public static void RemovePoint(HashSet<Vector2I> collection, Vector2I position)
    {
        if (collection.Contains(position))
        {
            collection.Remove(position);
        }     
        else
        {
            GD.Print("HashSet does not contain the position " + position);
        }      
    }

    public static Vector2I GetRandomPosition(HashSet<Vector2I> positions)
    {
        int index = Random.rng.RandiRange(0, positions.Count - 1);
        Vector2I randomPosition = positions.ElementAt(index);
        return randomPosition;
    }

    public void InitializeActors(HashSet<Vector2I> validPositions)
    {
        Vector2I enemyPosition = GetRandomPosition(validPositions);
        enemy.GridPosition = enemyPosition;
        enemy.Position = tilemapVisualizer.MapToLocal(enemyPosition);
        minimapVisualizer.UpdateActor(Actor.Type.Enemy, enemyPosition, enemyPosition);

        Vector2I startPosition = GetRandomPosition(validPositions);
        player.GridPosition = startPosition;
        player.Position = tilemapVisualizer.MapToLocal(startPosition);
        minimapVisualizer.UpdateActor(Actor.Type.Player, startPosition, startPosition);
        
        minimapVisualizer.UpdateMinimap();
    }

    public void InitializeStairs(HashSet<Vector2I> validPositions)
    {
        Vector2I startPosition = GetRandomPosition(validPositions);
        GridManager.SetStairLocation(startPosition);
        tilemapVisualizer.PaintTile(1, startPosition, 1, Vector2I.Zero);
    }
}
