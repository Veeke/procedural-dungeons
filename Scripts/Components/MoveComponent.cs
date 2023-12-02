using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Security.AccessControl;
using Godot;

public partial class MoveComponent : Node
{
    [Export]
    private Actor actor;
    [Export]
    private AnimatorComponent animator;
    [Export]
    private int walkSpeed;
    [Export]
    private int sprintSpeed;
    private int moveSpeed;
    private GameEvents gameEvents;
    public bool IsMoving {get; set;}

    public override void _Ready()
    {
        moveSpeed = walkSpeed;
        gameEvents = GetNode<GameEvents>("/root/GameEvents");
        gameEvents.Connect(GameEvents.SignalName.StairsReached, new Callable(this, MethodName.ResetActor));
    }

    public override void _PhysicsProcess(double delta)
    {
        SetMoveSpeed();
        if (IsMoving)
        {
            actor.Position = actor.Position.MoveToward(actor.WorldPosition, (float)delta * moveSpeed);

            if (actor.Position == actor.WorldPosition)
            {
                IsMoving = false;
                actor.CheckTileContent(actor.GridPosition);
            }
        }
    }

    public void ResetActor()
    {
        IsMoving = false;
        actor.Visible = true;
    }

    public void SetMoveSpeed()
    {
        if (Input.IsActionJustPressed("sprint"))
        {
            moveSpeed = sprintSpeed;       
        }
        if (Input.IsActionJustReleased("sprint"))
        {
            moveSpeed = walkSpeed;
        }
    }

    public void MoveOnGrid(Vector2I direction)
    {
        if (direction == Vector2I.Zero)
        {
            return;
        }

        animator.PlayMoveAnimation(direction);

        if (CanMoveInDirection(actor.GridPosition, direction))
        {    
            Vector2I newGridPosition = actor.GridPosition + direction;
            Tile destination = GridManager.map.GetCell(newGridPosition);

            GridManager.map.GetCell(actor.GridPosition).IsOccupied = false;
            destination.IsOccupied = true;
        
            if (actor.Visible != destination.IsVisible)
            {
                actor.Visible = destination.IsVisible;
            }

            actor.GridPosition = newGridPosition;
            actor.WorldPosition = GridManager.ConvertGridToWorld(actor.GridPosition);
            IsMoving = true;
        }
    }

    public static bool CanMoveInDirection(Vector2I gridPosition, Vector2I direction)
    {
        Vector2I newGridPosition = gridPosition + direction;
        if (!GridManager.IsValidCoord(newGridPosition))
        {
            return false;
        }

        Tile destination = GridManager.map.GetCell(newGridPosition);
        if (destination.IsOccupied || !destination.IsFloor())
        {
            return false;
        }

        // Prevents cutting corners when moving diagonally
        if (direction.X != 0 && direction.Y != 0)
        {
            Vector2I[] neighbourPositions = Directions.GetCardinalNeighbours(gridPosition);
            Tile[] cardinalNeighbours = GridManager.map.GetCells(neighbourPositions);
        
            if (direction == Directions.TopRight && (cardinalNeighbours[0].IsWall() || cardinalNeighbours[1].IsWall()))
            {
                return false;
            }
            else if (direction == Directions.BottomRight && (cardinalNeighbours[1].IsWall() || cardinalNeighbours[2].IsWall()))
            {
                return false;
            }
            else if (direction == Directions.BottomLeft && (cardinalNeighbours[2].IsWall() || cardinalNeighbours[3].IsWall()))
            {
                return false;
            }
            else if (direction == Directions.TopLeft && (cardinalNeighbours[3].IsWall() || cardinalNeighbours[0].IsWall()))
            {
                return false;
            }
        }
        return true;
    }
}
