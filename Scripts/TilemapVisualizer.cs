using Godot;
using System.Collections;

public partial class TilemapVisualizer : TileMap
{
    // This bool determines how the bitmask value is calculated
    // By default (true), floor tiles are empty (0) and wall tiles contain the transitions (1)
    // If set to false, this is inverted (used for the minimap for example)
    [Export]
    protected bool invertTileset = false;
    [Export]
    protected bool useTileVariants = false;
    [Export]
    protected int borderSize = 16;

    protected readonly Vector2I EmptyTileCoords = Vector2I.Zero;
    protected readonly Vector2I FilledTileCoords = new (5, 1);
    
    public void VisualizeMap(GridMap map)
    {
        Vector2I gridSize = map.GetGridSize();
        for (int y = -borderSize; y < gridSize.Y + borderSize; y++)
        {
            for (int x = -borderSize; x < gridSize.X + borderSize; x++)
            {
                Vector2I position = new(x, y);
                Vector2I atlasCoords = invertTileset ? EmptyTileCoords : FilledTileCoords;
                if (x > 0 && x < gridSize.X && y > 0 && y < gridSize.Y)
                {
                    atlasCoords = AutoTiling(map, position);
                }               
                PaintTile(0, position, 0, atlasCoords);
            }
        }
    }

    public virtual Vector2I AutoTiling(GridMap map, Vector2I position)
    {
        // Neighbours start at the above neighbour and go clockwise
        // Up -> Top Right -> Right -> Bottom Right -> Bottom -> Bottom Left -> Left -> Top Left
        // This means that cardinal neighbours have an even index and intercardinal neighbours have an odd index
        Vector2I[] neighbours = Directions.GetAllNeighbours(position);
        BitArray bitArray = new(8, false);

        if ((map.GetCell(position).IsFloor() && !invertTileset) || (!map.GetCell(position).IsFloor() && invertTileset))
        {
            return BitmaskToTile(0);
        }

        // Cardinal checks, skip corners for now
        bool isEdge = false;
        for (int i = 0; i < neighbours.Length; i += 2)
        {
            if (GridManager.IsValidCoord(neighbours[i]))
            {
                bitArray[i] = map.GetCell(neighbours[i]).IsFloor() == invertTileset;
            }         
            else isEdge = true;      
        }

        // Tiles at the edge of the map can't properly check neighbours, so I set them manually
        if (isEdge)
        {
            return invertTileset ? BitmaskToTile(0) : BitmaskToTile(255);
        }

        //Corner checks, only check if both adjacent neighbours are true
        for (int i = 1; i < neighbours.Length; i += 2)
        {
            if (bitArray[i - 1] && bitArray[i + 1 == neighbours.Length ? 0 : i + 1])
            {
                bitArray[i] = map.GetCell(neighbours[i]).IsFloor() == invertTileset;
            }
        }      
        int bitmask = BinaryToNumeral(bitArray);     
        return BitmaskToTile(bitmask);
    }

    public static int BinaryToNumeral(BitArray bitArray)
    {
        int[] result = new int[1];
        bitArray.CopyTo(result, 0);
        return result[0];
    }

    public virtual Vector2I BitmaskToTile(int bitmask)
    {   
        Vector2I atlasCoords = Vector2I.Zero;

        // Use a simplified dictionary if we're not using any tile variants
        if (!useTileVariants)
        {
            atlasCoords = BitmaskTable.BitmaskToAtlasCoords[bitmask];
        }  
        else if (BitmaskTable.BitmaskToTileVariant[bitmask].Length == 1)
        {
            atlasCoords = BitmaskTable.BitmaskToTileVariant[bitmask][0].AtlasCoords;
        }
        else
        {
            float random = GD.Randf();
            float total = 0;
            for (int i = 0; i < BitmaskTable.BitmaskToTileVariant[bitmask].Length; i++)
            {
                total += BitmaskTable.BitmaskToTileVariant[bitmask][i].Probability;
                if (total >= random)
                {
                    atlasCoords = BitmaskTable.BitmaskToTileVariant[bitmask][i].AtlasCoords;
                    break;
                }
            }
        }
        return atlasCoords;
    }

    public void PaintTile(int layer, Vector2I position, int tilesetIndex, Vector2I atlasCoords)
    {
        SetCell(layer, position, tilesetIndex, atlasCoords);
    }

    public void ResetTilemap()
    {
        for (int i = 0; i < GetLayersCount(); i++)
        {
            ClearLayer(i);
        }
    }
}
