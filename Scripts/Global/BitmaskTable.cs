using System.Collections.Generic;
using Godot;

public class BitmaskTable
{
    public readonly struct TileVariant
    {
        public Vector2I AtlasCoords {get;}
        public float Probability {get;}

        public TileVariant(Vector2I atlasCoords, float probability)
        {
            if (probability <= 0 || probability > 1)
                probability = 0;

            AtlasCoords = atlasCoords;
            Probability = probability;
        }
    }

    public static readonly Dictionary<int, Vector2I> BitmaskToAtlasCoords = new()
    {
        {0, new Vector2I (0, 0)},
        {4, new Vector2I (1, 0)},
        {92, new Vector2I (2, 0)},
        {112, new Vector2I (3, 0)},
        {28, new Vector2I (4, 0)},
        {124, new Vector2I (5, 0)},
        {116, new Vector2I (6, 0)},
        {64, new Vector2I (7, 0)},
        {20, new Vector2I (0, 1)},
        {84, new Vector2I (1, 1)},
        {87, new Vector2I (2, 1)},
        {221, new Vector2I (3, 1)},
        {127, new Vector2I (4, 1)},
        {255, new Vector2I (5, 1)},
        {245, new Vector2I (6, 1)},
        {80, new Vector2I (7, 1)},
        {29, new Vector2I (0, 2)},
        {117, new Vector2I (1, 2)},
        {85, new Vector2I (2, 2)},
        {95, new Vector2I (3, 2)},
        {247, new Vector2I (4, 2)},
        {215, new Vector2I (5, 2)},
        {209, new Vector2I (6, 2)},
        {1, new Vector2I (7, 2)},
        {23, new Vector2I (0, 3)},
        {213, new Vector2I (1, 3)},
        {81, new Vector2I (2, 3)},
        {31, new Vector2I (3, 3)},
        {253, new Vector2I (4, 3)},
        {125, new Vector2I (5, 3)},
        {113, new Vector2I (6, 3)},
        {16, new Vector2I (7, 3)},
        {21, new Vector2I (0, 4)},
        {69, new Vector2I (1, 4)},
        {93, new Vector2I (2, 4)},
        {119, new Vector2I (3, 4)},
        {223, new Vector2I (4, 4)},
        // Skip (5, 4), duplicate tile in blob tileset
        {241, new Vector2I (6, 4)},
        {17, new Vector2I (7, 4)},
        {5, new Vector2I (0, 5)},
        {68, new Vector2I (1, 5)},
        {71, new Vector2I (2, 5)},
        {193, new Vector2I (3, 5)},
        {7, new Vector2I (4, 5)},
        {199, new Vector2I (5, 5)},
        {197, new Vector2I (6, 5)},
        {65, new Vector2I (7, 5)}
    };
    public static readonly Dictionary<int, TileVariant[]> BitmaskToTileVariant = new()
    {
        {0, new[] {
            new TileVariant(new Vector2I (0, 0), 0.8f),
            new TileVariant(new Vector2I (0, 6), 0.1f),
            new TileVariant(new Vector2I (0, 7), 0.1f)}}, 
        {4, new[] {
            new TileVariant(new Vector2I (1, 0), 1)}}, 
        {92, new[] {
            new TileVariant(new Vector2I (2, 0), 1)}}, 
        {112, new[] {
            new TileVariant(new Vector2I (3, 0), 1)}}, 
        {28, new[] {
            new TileVariant(new Vector2I (4, 0), 1)}}, 
        {124, new[] {
            new TileVariant(new Vector2I (5, 0), 0.35f),
            new TileVariant(new Vector2I (2, 6), 0.35f),
            new TileVariant(new Vector2I (3, 6), 0.3f)}}, 
        {116, new[] {
            new TileVariant(new Vector2I (6, 0), 1)}}, 
        {64, new[] {
            new TileVariant(new Vector2I (7, 0), 1)}},
        {20, new[] {
            new TileVariant(new Vector2I (0, 1), 1)}}, 
        {84, new[] {
            new TileVariant(new Vector2I (1, 1), 1)}}, 
        {87, new[] {
            new TileVariant(new Vector2I (2, 1), 1)}}, 
        {221, new[] {
            new TileVariant(new Vector2I (3, 1), 1)}}, 
        {127, new[] {
            new TileVariant(new Vector2I (4, 1), 1)}}, 
        {255, new[] {
            new TileVariant(new Vector2I (5, 1), 0.6f),
            new TileVariant(new Vector2I (1, 6), 0.05f),
            new TileVariant(new Vector2I (1, 7), 0.35f)}}, 
        {245, new[] {
            new TileVariant(new Vector2I (6, 1), 1)}}, 
        {80, new[] {
            new TileVariant(new Vector2I (7, 1), 1)}},
        {29, new[] {
            new TileVariant(new Vector2I (0, 2), 1)}}, 
        {117, new[] {
            new TileVariant(new Vector2I (1, 2), 1)}}, 
        {85, new[] {
            new TileVariant(new Vector2I (2, 2), 1)}}, 
        {95, new[] {
            new TileVariant(new Vector2I (3, 2), 1)}}, 
        {247, new[] {
            new TileVariant(new Vector2I (4, 2), 1)}}, 
        {215, new[] {
            new TileVariant(new Vector2I (5, 2), 1)}}, 
        {209, new[] {
            new TileVariant(new Vector2I (6, 2), 1)}}, 
        {1, new[] {
            new TileVariant(new Vector2I (7, 2), 1)}},
        {23, new[] {
            new TileVariant(new Vector2I (0, 3), 1)}}, 
        {213, new[] {
            new TileVariant(new Vector2I (1, 3), 1)}}, 
        {81, new[] {
            new TileVariant(new Vector2I (2, 3), 1)}}, 
        {31, new[] {
            new TileVariant(new Vector2I (3, 3), 0.35f),
            new TileVariant(new Vector2I (4, 6), 0.35f),
            new TileVariant(new Vector2I (4, 7), 0.3f)}}, 
        {253, new[] {
            new TileVariant(new Vector2I (4, 3), 1)}}, 
        {125, new[] {
            new TileVariant(new Vector2I (5, 3), 1)}}, 
        {113, new[] {
            new TileVariant(new Vector2I (6, 3), 1)}}, 
        {16, new[] {
            new TileVariant(new Vector2I (7, 3), 1)}},
        {21, new[] {
            new TileVariant(new Vector2I (0, 4), 1)}}, 
        {69, new[] {
            new TileVariant(new Vector2I (1, 4), 1)}}, 
        {93, new[] {
            new TileVariant(new Vector2I (2, 4), 1)}}, 
        {119, new[] {
            new TileVariant(new Vector2I (3, 4), 1)}}, 
        {223, new[] {
            new TileVariant(new Vector2I (4, 4), 1)}}, 
        {241, new[] {
            new TileVariant(new Vector2I (6, 4), 0.35f),
            new TileVariant(new Vector2I (5, 6), 0.35f),
            new TileVariant(new Vector2I (5, 7), 0.3f)}}, 
        {17, new[] {
            new TileVariant(new Vector2I (7, 4), 1)}},
        {5, new[] {
            new TileVariant(new Vector2I (0, 5), 1)}}, 
        {68, new[] {
            new TileVariant(new Vector2I (1, 5), 1)}}, 
        {71, new[] {
            new TileVariant(new Vector2I (2, 5), 1)}}, 
        {193, new[] {
            new TileVariant(new Vector2I (3, 5), 1)}}, 
        {7, new[] {
            new TileVariant(new Vector2I (4, 5), 1)}}, 
        {199, new[] {
            new TileVariant(new Vector2I (5, 5), 0.35f),
            new TileVariant(new Vector2I (2, 7), 0.35f),
            new TileVariant(new Vector2I (3, 7), 0.3f)}}, 
        {197, new[] {
            new TileVariant(new Vector2I (6, 5), 1)}}, 
        {65, new[] {
            new TileVariant(new Vector2I (7, 5), 1)}}
    };
}

   
