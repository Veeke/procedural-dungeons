using Godot;

public partial class AnimatorComponent : Node
{
    [Export]
    private AnimatedSprite2D sprite;

    public void PlayMoveAnimation(Vector2I direction)
    {
        if (direction == Directions.Right)
            sprite.Play("right");
        else if (direction == Directions.Left)
            sprite.Play("left");
        else if (direction == Directions.Up)
            sprite.Play("up");
        else if (direction == Directions.Down)
            sprite.Play("down");
        else if (direction == Directions.TopRight)
            sprite.Play("top right");
        else if (direction == Directions.TopLeft)
            sprite.Play("top left");    
        else if (direction == Directions.BottomRight)
            sprite.Play("bottom right");    
        else if (direction == Directions.BottomLeft)
            sprite.Play("bottom left");
    }
}
