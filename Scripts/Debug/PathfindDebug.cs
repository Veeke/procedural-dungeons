using Godot;
using System.Collections.Generic;

public partial class PathfindDebug : Node2D
{
	private List<Vector2I> path = new();
    private Vector2I playerPos;
    public override void _Draw()
    {
        DrawPath();
        DrawPlayerPos();
    }

    public void DrawPath()
    {
        for (int i = 0; i < path.Count; i++)
        {
            DrawCircle(GridManager.ConvertGridToWorld(path[i]), 2, new Color(1.0f, 1.0f, 1.0f));	
        }
    }

    public void DrawPlayerPos()
    {
        if (playerPos!= Vector2I.Zero)
        {
            DrawCircle(GridManager.ConvertGridToWorld(playerPos), 4, new Color(1.0f, 0f, 0f));
        } 
    }

	public void SetPath(List<Vector2I> newPath)
	{
		path = newPath;
		QueueRedraw();
	}

    public void SetPlayerPos(Vector2I position)
    {
        playerPos = position;
        QueueRedraw();
    }
}
