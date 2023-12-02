using Godot;

public partial class MoveInputComponent : Node
{   
    [Export]
    private MoveComponent moveComponent;
    private Vector2I inputDir;
    
    public override void _PhysicsProcess(double delta)
    {
        inputDir.X = (Input.IsActionPressed("right") ? 1 : 0) - (Input.IsActionPressed("left") ? 1 : 0);
        inputDir.Y = (Input.IsActionPressed("down") ? 1 : 0) - (Input.IsActionPressed("up") ? 1 : 0);

        if (!moveComponent.IsMoving && inputDir != Vector2I.Zero)
        {
            moveComponent.MoveOnGrid(inputDir);
        }  
    }
}
