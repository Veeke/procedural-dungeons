using Godot;
using System.Collections.Generic;

public partial class ProceduralGenerationAlgorithms
{
    public static List<Rect2I> BinarySpacePartioning(
        Vector2I gridSize, int horizontalChunks, int verticalChunks, Vector2I minRoomSize, Vector2I maxRoomSize, int minDistance)
    {
        List<Rect2I> chunkList = new();
        List<Rect2I> roomList = new();

        int chunkSizeX = (gridSize.X - 2 * minDistance) / horizontalChunks;
        int chunkSizeY = (gridSize.Y - 2 * minDistance) / verticalChunks;

        Vector2I sectorSize = new(chunkSizeX, chunkSizeY);

        for (int y = 0; y < verticalChunks; y++)
        {
            for (int x = 0; x < horizontalChunks; x++)
            {
                Vector2I sectorPosition = new(minDistance + x * chunkSizeX, minDistance + y * chunkSizeY);
                Rect2I sectorRect = new(sectorPosition, sectorSize);
                chunkList.Add(sectorRect);
            }          
        }

        foreach (Rect2I chunk in chunkList)
        {
            int widthAvailable = Mathf.Min(maxRoomSize.X, chunkSizeX - minDistance * 2);
            int heightAvailable = Mathf.Min(maxRoomSize.Y, chunkSizeY - minDistance * 2);

            int roomSizeX = Random.rng.RandiRange(minRoomSize.X, widthAvailable);
            int roomSizeY = Random.rng.RandiRange(minRoomSize.Y, heightAvailable);

            int roomPositionX = Random.rng.RandiRange(
                chunk.Position.X + minDistance, 
                chunk.End.X - 1 - roomSizeX - minDistance);
            int roomPositionY = Random.rng.RandiRange(
                chunk.Position.Y + minDistance, 
                chunk.End.Y - 1 - roomSizeY - minDistance);
    
            Vector2I roomSize = new(roomSizeX, roomSizeY);
            Vector2I roomPosition = new(roomPositionX, roomPositionY);

            Rect2I room = new(roomPosition, roomSize);
            roomList.Add(room);
        } 
        return roomList;
    }
}
