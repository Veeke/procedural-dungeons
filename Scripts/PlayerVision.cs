using System.Collections;
using Godot;

public partial class PlayerVision : TilemapVisualizer
{
    // Conceptually, it doesn't make sense for visionRange to extend beyond half the tiles the player can see on their screen
    [Export (PropertyHint.Range, "1, 8,")]
    private int visionRange = 8;
    [Export]
    private bool hasFog = false;
    private GameEvents gameEvents;

    public override void _Ready()
    {
        gameEvents = GetNode<GameEvents>("/root/GameEvents");
        gameEvents.Connect(GameEvents.SignalName.PlayerMoved, new Callable(this, MethodName.UpdateVision));

        if (hasFog) 
        {
            gameEvents.Connect(GameEvents.SignalName.StairsReached, new Callable(this, MethodName.ResetFog));
        }
    }

    public void UpdateVision(Vector2I origin)
    {
        GridManager.ResetVision(origin, visionRange);
        FieldOfView.ComputeFOV(origin, visionRange);
        gameEvents.EmitSignal(GameEvents.SignalName.VisionUpdated);

        if (hasFog)
        {
            VisualizeFog(origin);
        }
        else
        {
            ClearLayer(0);
        }
    }

    public void VisualizeFog(Vector2I origin)
    {
        // Only update the tiles that are currently or used to be in vision range
        int range = visionRange + 2;
   
        for (int y = origin.Y - range; y <= origin.Y + range; y++)
        {
            for (int x = origin.X - range; x <= origin.X + range; x++)
            {
                Vector2I position = new(x, y);
                if (GridManager.IsValidCoord(position))
                {
                    Vector2I atlasCoords = AutoTiling(GridManager.map, position);
                    PaintTile(0, position, 0, atlasCoords);
                }
                else
                {
                    PaintTile(0, position, 0, FilledTileCoords);
                }
            }
        }
    }

    public void ResetFog()
    {
        Vector2I gridSize = GridManager.map.GetGridSize();
        for (int y = -borderSize; y < gridSize.Y + borderSize; y++)
        {
            for (int x = -borderSize; x < gridSize.X + borderSize; x++)
            {
                Vector2I position = new(x, y);
                if (GridManager.IsValidCoord(position))
                {
                    PaintTile(0, position, 0, FilledTileCoords);
                }
            }
        }
    }

    public override Vector2I AutoTiling(GridMap map, Vector2I position)
    {
        // Neighbours start at the above neighbour and go clockwise
        // Up -> Top Right -> Right -> Bottom Right -> Bottom -> Bottom Left -> Left -> Top Left
        // This means that cardinal neighbours have an even index and intercardinal neighbours have an odd index
        Vector2I[] neighbours = Directions.GetAllNeighbours(position);
        BitArray bitArray = new(8, false);

        if (map.GetCell(position).IsVisible)
        {
            return BitmaskToTile(0);
        }

        // Cardinal checks, skip corners for now
        bool isEdge = false;
        for (int i = 0; i < neighbours.Length; i += 2)
        {
            if (GridManager.IsValidCoord(neighbours[i]))
            {
                bitArray[i] = !map.GetCell(neighbours[i]).IsVisible;
            }         
            else isEdge = true;      
        }

        // Tiles at the edge of the map can't properly check neighbours, so I set them manually
        if (isEdge)
        {
            return BitmaskToTile(255);
        }

        //Corner checks, only check if both adjacent neighbours are true
        for (int i = 1; i < neighbours.Length; i += 2)
        {
            if (bitArray[i - 1] && bitArray[i + 1 == neighbours.Length ? 0 : i + 1])
            {
                bitArray[i] = !map.GetCell(neighbours[i]).IsVisible;
            }
        }
        int bitmask = BinaryToNumeral(bitArray);     
        return BitmaskToTile(bitmask);
    }
}
