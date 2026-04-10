using Godot;

public partial class SquareGameData : GodotObject
{
    public bool IsTakingSquare;
    public bool IsCastlingSquare;
    public EnPassantDirection? EnPassantDirection;
    public bool IsOccupied;
    public Piece Occupying2DPiece;

    public SquareGameData()
    {
        IsTakingSquare = false;
        IsCastlingSquare = false;
        IsOccupied = false;
    }
}