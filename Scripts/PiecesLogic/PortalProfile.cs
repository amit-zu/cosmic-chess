using System;
using Godot;
using static GameManager;
public partial class PortalProfile : BoardEdgeItemProfile
{
    public override bool Transform(EdgeCoordinate connectedItemEdgeItemCoordinate, Direction portalDirection, ref SquareCoordinate squareCoordinate, ref Vector2 movementVector, ref int bounces)
    {
        bool shouldTransform = ShouldTransform(portalDirection, movementVector);

        if (shouldTransform)
        {
            // Entry & exit facings as vectors
            Vector2 Fin = Util.GetVectorFromDirection(portalDirection); // entry facing
            Vector2 Tin = new Vector2(Fin.Y, -Fin.X); // tangent (90° clockwise)
            Vector2 Fout = Util.GetVectorFromDirection(Util.GetOppositeDirection(connectedItemEdgeItemCoordinate.Direction)); // exit facing
            Vector2 Tout = new Vector2(Fout.Y, -Fout.X);

            // Components relative to entry frame
            int a = Math.Sign(movementVector.Dot(Fin)); // forward
            int b = Math.Sign(movementVector.Dot(Tin)); // tangent

            // Recompose relative to exit frame, flipping forward
            Vector2 vOut = (-a) * Fout + b * Tout;

            // Clamp to {-1,0,1} components
            vOut.X = Math.Clamp((int)vOut.X, -1, 1);
            vOut.Y = Math.Clamp((int)vOut.Y, -1, 1);

            movementVector = vOut;

            squareCoordinate = GetSquareCoordinate(connectedItemEdgeItemCoordinate.File, connectedItemEdgeItemCoordinate.Rank);
        }

        return shouldTransform;
    } // TODO this is still bugged
}