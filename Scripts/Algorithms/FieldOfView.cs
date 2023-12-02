using Godot;

public partial class FieldOfView
{
    public static void ComputeFOV(Vector2I origin, int rangeLimit)
    {
        GridManager.SetVisible(origin.X, origin.Y, true);
        GridManager.SetExplored(origin.X, origin.Y, true);

        for(int quadrant = 0; quadrant < 4; quadrant++)
        {
            Scan(quadrant, origin, rangeLimit, 1, -1, 1);
        }
    }

    public static void Scan(int quadrant, Vector2I origin, int visionRange, int y, float startSlope, float endSlope)
    {
        bool? wasWall = null;
        
        // startSlope and endSlope are the slopes of the lines that define our field of view within the current quadrant
        // We start with 45 degree slopes (startSlope = -1, endSlope = 1)
        // We scan all the tiles that fall within this field of view row by row, so first we determine how many tiles we need to check
        // The formula for a line is y = ax + b, where a is the slope of the line (y / x) and b = 0 because the starting point is always the origin
        // Since y = ax → x = y/a, we can calculate the start and end of a row (x) by dividing the scan's depth (y) by the corresponding slope, then rounding the result

        int rowStart = startSlope == -1 ? -y : RoundUp(y / startSlope);
        int rowEnd = endSlope == 1 ? y : RoundDown(y / endSlope);

        for (int x = rowStart; x <= rowEnd; x++)
        {             
            int ox = origin.X, oy = origin.Y;

            // Convert local coordinates to grid coordinates based on the quadrant we're currently scanning
            switch(quadrant)
            {
                case 0: ox += x; oy -= y; break;    //north
                case 1: ox += x; oy += y; break;    //south
                case 2: ox += y; oy += x; break;    //east
                case 3: ox -= y; oy += x; break;    //west
            }
 
            bool isWall = GridManager.BlocksLight(ox, oy);

            // Enforce symmetry (for any given points A and B on the grid, if A can see B, B should be able to see A)
            if (y <= visionRange && !isWall && IsSymmetric(x, y, startSlope, endSlope))
            {
                GridManager.SetVisible(ox, oy, true);
                GridManager.SetExplored(ox, oy, true);
            }

            // We found a transition from a wall tile to an empty tile
            if (wasWall == true && isWall == false) 
            {
                // We adjust our search by drawing startSlope as a line from the origin through the right corner of the wall tile
                // The origin lies on the center of the tile, wall tiles are treated as if they have bevelled corners (diamond shape)
                // Thus, the corner we need is offset by -0.5 in the x direction relative to our current tile's center, so the slope between these points is y / (x - 0.5)
                // We multiply everything by 2 to avoid floating point precision → 2y / 2(x - 0.5) → 2y / (2x - 1)
                startSlope = 2.0f * y / (2.0f * x - 1);
            }

            // We found a transition an empty tile to a wall tile
            if (wasWall == false && isWall == true) 
            {     
                if (y != visionRange)
                {
                    // We adjust our search by drawing a new endSlope as a line from the origin through the left corner of the wall tile
                    // As the current tile is said wall tile, the offset and math end up being the exact same as in the previous case
                    // We then recursively call this function to continue our search within this new field of view
                    // We do not overwrite endSlope because we continue with its original value once we leave the recursion
                    float newEndSlope = 2.0f * y / (2.0f * x - 1); 
                    Scan(quadrant, origin, visionRange, y + 1, startSlope, newEndSlope);                
                }                       
            }
            wasWall = isWall;                    
        }

        // If we're at the end of the row and the last tile was clear, move on to the next row
        if (wasWall == false && y != visionRange)
        {
            Scan(quadrant, origin, visionRange, y + 1, startSlope, endSlope);
        }
    }
    
    public static int RoundUp(float number)
    {
        return (int)Mathf.Floor(number + 0.5);
    }

    public static int RoundDown(float number)
    {
        return (int)Mathf.Ceil(number - 0.5);
    }

    public static bool IsSymmetric(int x, int y, float startSlope, float endSlope)
    {
        return x >= y * startSlope && x <= y * endSlope;
    }
}
