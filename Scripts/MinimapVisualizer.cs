using System;
using System.Collections.Generic;
using Godot;

public partial class MinimapVisualizer : TilemapVisualizer
{
    private GameEvents gameEvents;
    readonly Vector2I stairsAtlasCoords = new(0, 6);
    readonly Vector2I playerAtlasCoords = new(1, 6);
    readonly Vector2I enemyAtlasCoords = new(2, 6);
    readonly Vector2I itemAtlasCoords = new(3, 6);
    
    public override void _Ready()
    {
        gameEvents = GetNode<GameEvents>("/root/GameEvents");
        gameEvents.Connect(GameEvents.SignalName.ActorMoved, new Callable(this, MethodName.UpdateActor));
        gameEvents.Connect(GameEvents.SignalName.VisionUpdated, new Callable(this, MethodName.UpdateMinimap));
    }

    public void UpdateActor(Actor.Type actorType, Vector2I oldPosition, Vector2I newPosition)
    {
        // Update the actor's position on the minimap
        if (oldPosition != newPosition)
        {
            EraseCell(2, oldPosition);
        }

        if (actorType == Actor.Type.Player)
        {
            PaintTile(2, newPosition, 0, playerAtlasCoords);
        }
        
        if (GridManager.map.GetCell(newPosition).IsVisible)
        {
            if (actorType == Actor.Type.Enemy)
            {
                PaintTile(2, newPosition, 0, enemyAtlasCoords);
            }
            else if (actorType == Actor.Type.Item)
            {
                PaintTile(2, newPosition, 0, itemAtlasCoords);
            }
        }     
    }

    public void UpdateMinimap()
    {
        Godot.Collections.Array<Vector2I> filledCells = GetUsedCells(0);
        bool stairsPlaced = GetUsedCells(1).Count > 0;

        // Add newly visible tiles to the minimap
        Vector2I gridSize = GridManager.map.GetGridSize();
        for (int y = 0; y < gridSize.Y; y++)
        {
            for (int x = 0; x < gridSize.X; x++)
            {
                Vector2I position = new(x, y);
                if (!filledCells.Contains(position) && GridManager.map.GetCell(position).IsExplored)
                {
                    Vector2I atlasCoords = AutoTiling(GridManager.map, position);
                    PaintTile(0, position, 0, atlasCoords);
                    {
                        if (GridManager.map.GetCell(position).IsStairs() && !stairsPlaced)
                        {
                            PaintTile(1, position, 0, stairsAtlasCoords);
                        }
                    }
                }                      
            }
        }
    }

    public override Vector2I BitmaskToTile(int bitmask)
    {   
        return BitmaskTable.BitmaskToAtlasCoords[bitmask];
    }
}
