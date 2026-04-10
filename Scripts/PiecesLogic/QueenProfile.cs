using System.Collections.Generic;

public partial class QueenProfile : PieceProfile
{
	public QueenProfile()
	{
		Points = 9;
	}

	public override List<SquareCoordinate> GetLegalSquares(BoardState boardState, SquareCoordinate currentSquare, bool hasMoved)
	{
		List<SquareCoordinate> legalSquares = new List<SquareCoordinate>();
		Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

		legalSquares.AddRange(GameManager.GetPieceProfile<RookProfile>().GetLegalSquares(boardState, currentSquare, hasMoved));
		legalSquares.AddRange(GameManager.GetPieceProfile<BishopProfile>().GetLegalSquares(boardState, currentSquare, hasMoved));


		return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), legalSquares);
	}

	public override List<SquareCoordinate> GetTakingSquares(BoardState boardState, SquareCoordinate currentSquare)
	{
		List<SquareCoordinate> takingSquares = new List<SquareCoordinate>();
		Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

		takingSquares.AddRange(GameManager.GetPieceProfile<RookProfile>().GetTakingSquares(boardState, currentSquare));
		takingSquares.AddRange(GameManager.GetPieceProfile<BishopProfile>().GetTakingSquares(boardState, currentSquare));

		return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), takingSquares);
	}


	public override List<SquareCoordinate> GetCheckingSquares(BoardState boardState, SquareCoordinate currentSquare)
	{
		List<SquareCoordinate> checkingSquares = new List<SquareCoordinate>();

		checkingSquares.AddRange(GameManager.GetPieceProfile<RookProfile>().GetCheckingSquares(boardState, currentSquare));
		checkingSquares.AddRange(GameManager.GetPieceProfile<BishopProfile>().GetCheckingSquares(boardState, currentSquare));

		return checkingSquares;
	}

	public override string ToString()
	{
		return "Queen";
	}
}
