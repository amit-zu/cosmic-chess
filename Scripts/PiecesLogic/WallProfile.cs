using Godot;

public partial class WallProfile : BoardEdgeItemProfile
{
    public override bool Transform(EdgeCoordinate connectedItemEdgeItemCoordinate, Direction wallDirection, ref SquareCoordinate squareCoordinate, ref Vector2 movementVector, ref int bounces)
    {
        bool shouldTransform = ShouldTransform(wallDirection, movementVector);

        if (shouldTransform)
        {
            bounces = 0;
        }

        return shouldTransform;
    }
}
