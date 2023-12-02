using System.Diagnostics;
using Godot;

public partial class Actor : Node2D
{
    public enum Type { Player, Enemy, Item }
    [Export]
    private Type ActorType;

    private Vector2I _gridPosition;
    public Vector2I GridPosition
    {
        get {return _gridPosition;}
        set
        {
            OnPositionChanged(_gridPosition, value);
            _gridPosition = value;
        }
    }
    public Vector2I WorldPosition {get; set;}
    protected GameEvents gameEvents;

    public override void _Ready()
    {
        gameEvents = GetNode<GameEvents>("/root/GameEvents");
    }

    public virtual void OnPositionChanged(Vector2I oldPosition, Vector2I newPosition)
    {
        gameEvents.EmitSignal(GameEvents.SignalName.ActorMoved, (int)ActorType, oldPosition, newPosition);
    }

    public Type GetActorType()
    {
        return ActorType;
    }

    public virtual void CheckTileContent(Vector2I position)
    {
    }
}
