using System.Collections.Generic;
using static GameManager;

public partial class RookProfile : PieceProfile
{
	public RookProfile()
	{
		Points = 5;
	}

	public override List<SquareCoordinate> GetLegalSquares(BoardState boardState, SquareCoordinate currentSquare, bool hasMoved)
	{
		List<SquareCoordinate> legalSquares = new List<SquareCoordinate>();
		Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

		TraverseInDirection(currentSquare.File, currentSquare.Rank, boardState, legalSquares, new Godot.Vector2(0, 1));
		TraverseInDirection(currentSquare.File, currentSquare.Rank, boardState, legalSquares, new Godot.Vector2(0, -1));
		TraverseInDirection(currentSquare.File, currentSquare.Rank, boardState, legalSquares, new Godot.Vector2(-1, 0));
		TraverseInDirection(currentSquare.File, currentSquare.Rank, boardState, legalSquares, new Godot.Vector2(1, 0));

		return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), legalSquares);
	}

	public override List<SquareCoordinate> GetTakingSquares(BoardState boardState, SquareCoordinate currentSquare)
	{
		List<SquareCoordinate> takingSquares = new List<SquareCoordinate>();
		Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

		CaptureInDirection(currentSquare.File, currentSquare.Rank, color, boardState, takingSquares, new Godot.Vector2(0, 1));
		CaptureInDirection(currentSquare.File, currentSquare.Rank, color, boardState, takingSquares, new Godot.Vector2(0, -1));
		CaptureInDirection(currentSquare.File, currentSquare.Rank, color, boardState, takingSquares, new Godot.Vector2(-1, 0));
		CaptureInDirection(currentSquare.File, currentSquare.Rank, color, boardState, takingSquares, new Godot.Vector2(1, 0));

		return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), takingSquares);
	}

	public override List<SquareCoordinate> GetCheckingSquares(BoardState boardState, SquareCoordinate currentSquare)
	{
		List<SquareCoordinate> checkingSquares = new List<SquareCoordinate>();
		Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

		CheckInDirection(currentSquare.File, currentSquare.Rank, color, boardState, checkingSquares, new Godot.Vector2(0, 1));
		CheckInDirection(currentSquare.File, currentSquare.Rank, color, boardState, checkingSquares, new Godot.Vector2(0, -1));
		CheckInDirection(currentSquare.File, currentSquare.Rank, color, boardState, checkingSquares, new Godot.Vector2(-1, 0));
		CheckInDirection(currentSquare.File, currentSquare.Rank, color, boardState, checkingSquares, new Godot.Vector2(1, 0));

		return checkingSquares;
	}

	public override string ToString()
	{
		return "Rook";
	}
}
