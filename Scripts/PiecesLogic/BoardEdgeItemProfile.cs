using Godot;

public abstract partial class BoardEdgeItemProfile : GodotObject
{
    public abstract bool Transform(EdgeCoordinate connectedItemEdgeItemCoordinate, Direction boardEdgeItemDirection, ref SquareCoordinate squareCoordinate, ref Vector2 movementVector, ref int bounces);

    public bool ShouldTransform(Direction boardEdgeItemDirection, Vector2 movementVector)
    {
        switch (boardEdgeItemDirection)
        {
            case Direction.Up:
                if (movementVector.Y == -1)
                    return true;
                break;
            case Direction.Down:
                if (movementVector.Y == 1)
                    return true;
                break;
            case Direction.Left:
                if (movementVector.X == 1)
                    return true;
                break;
            case Direction.Right:
                if (movementVector.X == -1)
                    return true;
                break;
        }

        return false;
    }
}
