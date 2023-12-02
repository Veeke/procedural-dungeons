using Godot;

public partial class Player : Actor
{
    public override void CheckTileContent(Vector2I position)
    {
        base.CheckTileContent(position);
        if (GridManager.map.GetCell(position).ContentType == Tile.Content.Stairs)
        {
            gameEvents.EmitSignal(GameEvents.SignalName.StairsReached);
        }
    }

    public override void OnPositionChanged(Vector2I oldPosition, Vector2I newPosition)
    {
        base.OnPositionChanged(oldPosition, newPosition);
        GridManager.PlayerPosition = newPosition;
        gameEvents.EmitSignal(GameEvents.SignalName.PlayerMoved, newPosition); 
    }
}
