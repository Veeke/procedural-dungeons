using Godot;

public partial class GameEvents : Node
{
    [Signal]
    public delegate void ActorMovedEventHandler(Actor.Type actor, Vector2I oldPosition, Vector2I newPosition);
    [Signal]
    public delegate void PlayerMovedEventHandler(Vector2I gridPosition);
    [Signal]
    public delegate void VisionUpdatedEventHandler();
    [Signal]
    public delegate void StairsReachedEventHandler();
}
