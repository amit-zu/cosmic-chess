using System.Collections.Generic;
using Godot;
using static GameManager;

public partial class BishopProfile : PieceProfile
{
    public BishopProfile()
    {
        Points = 3;
    }

    public override List<SquareCoordinate> GetLegalSquares(BoardState boardState, SquareCoordinate currentSquare, bool hasMoved)
    {
        List<SquareCoordinate> legalSquares = new List<SquareCoordinate>();
        Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

        TraverseInDirection(currentSquare.File, currentSquare.Rank, boardState, legalSquares, new Godot.Vector2(1, 1));
        TraverseInDirection(currentSquare.File, currentSquare.Rank, boardState, legalSquares, new Godot.Vector2(-1, 1));
        TraverseInDirection(currentSquare.File, currentSquare.Rank, boardState, legalSquares, new Godot.Vector2(1, -1));
        TraverseInDirection(currentSquare.File, currentSquare.Rank, boardState, legalSquares, new Godot.Vector2(-1, -1));

        return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), legalSquares);
    }

    public override List<SquareCoordinate> GetTakingSquares(BoardState boardState, SquareCoordinate currentSquare)
    {
        List<SquareCoordinate> takingSquares = new List<SquareCoordinate>();
        Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

        CaptureInDirection(currentSquare.File, currentSquare.Rank, color, boardState, takingSquares, new Vector2(1, 1));
        CaptureInDirection(currentSquare.File, currentSquare.Rank, color, boardState, takingSquares, new Vector2(1, -1));
        CaptureInDirection(currentSquare.File, currentSquare.Rank, color, boardState, takingSquares, new Vector2(-1, 1));
        CaptureInDirection(currentSquare.File, currentSquare.Rank, color, boardState, takingSquares, new Vector2(-1, -1));

        return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), takingSquares);
    }


    public override List<SquareCoordinate> GetCheckingSquares(BoardState boardState, SquareCoordinate currentSquare)
    {
        List<SquareCoordinate> checkingSquares = new List<SquareCoordinate>();
        Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

        CheckInDirection(currentSquare.File, currentSquare.Rank, color, boardState, checkingSquares, new Vector2(1, 1));
        CheckInDirection(currentSquare.File, currentSquare.Rank, color, boardState, checkingSquares, new Vector2(1, -1));
        CheckInDirection(currentSquare.File, currentSquare.Rank, color, boardState, checkingSquares, new Vector2(-1, 1));
        CheckInDirection(currentSquare.File, currentSquare.Rank, color, boardState, checkingSquares, new Vector2(-1, -1));

        return checkingSquares;
    }

    public override string ToString()
    {
        return "Bishop";
    }
}