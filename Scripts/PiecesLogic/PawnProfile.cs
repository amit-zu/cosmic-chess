using System.Collections.Generic;

using static GameManager;

public partial class PawnProfile : PieceProfile
{
	public PawnProfile()
	{
		Points = 1;
	}

	public override List<SquareCoordinate> GetLegalSquares(BoardState boardState, SquareCoordinate currentSquare, bool hasMoved)
	{
		List<SquareCoordinate> legalSquares = new List<SquareCoordinate>();
		Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

		if (color == Color.WHITE && currentSquare.Rank <= Rank.SEVEN)
		{
			var nextSquare = GetSquareCoordinate(currentSquare.File, currentSquare.Rank + 1);

			if (boardState.GetBoardItemAt(nextSquare) == null)
			{
				if (!hasMoved)
				{
					var secondNextSquare = GetSquareCoordinate(currentSquare.File, currentSquare.Rank + 2);

					if (boardState.GetBoardItemAt(secondNextSquare) == null)
					{
						legalSquares.Add(secondNextSquare);
					}
				}

				legalSquares.Add(nextSquare);
			}
		}
		else if (color == Color.BLACK && currentSquare.Rank >= Rank.TWO)
		{
			var nextSquare = GetSquareCoordinate(currentSquare.File, currentSquare.Rank - 1);

			if (boardState.GetBoardItemAt(nextSquare) == null)
			{
				if (!hasMoved)
				{
					var secondNextSquare = GetSquareCoordinate(currentSquare.File, currentSquare.Rank - 2);
					if (boardState.GetBoardItemAt(secondNextSquare) == null)
					{
						legalSquares.Add(secondNextSquare);
					}
				}

				legalSquares.Add(nextSquare);
			}
		}

		return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), legalSquares);
	}

	public override List<SquareCoordinate> GetTakingSquares(BoardState boardState, SquareCoordinate currentSquare)
	{
		List<SquareCoordinate> takingSquares = new List<SquareCoordinate>();

		Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

		if (color == Color.WHITE && currentSquare.Rank <= Rank.SEVEN)
		{
			if (currentSquare.File <= File.G)
			{
				var rightTakingSquare = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank + 1);

				if (boardState.GetBoardItemAt(rightTakingSquare) != null && boardState.GetBoardItemAt(rightTakingSquare).Value.Item2 != Color.WHITE
				&& !(boardState.GetBoardItemAt(rightTakingSquare).Value.Item1 == GameManager.GetPieceProfile<KingProfile>()))
				{
					takingSquares.Add(rightTakingSquare);
				}
			}
			if (currentSquare.File >= File.B)
			{
				var leftTakingSquare = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank + 1);

				if (boardState.GetBoardItemAt(leftTakingSquare) != null && boardState.GetBoardItemAt(leftTakingSquare).Value.Item2 != Color.WHITE
				&& !(boardState.GetBoardItemAt(leftTakingSquare).Value.Item1 == GameManager.GetPieceProfile<KingProfile>()))
				{
					takingSquares.Add(leftTakingSquare);
				}
			}
		}
		else if (color == Color.BLACK && currentSquare.Rank >= Rank.TWO)
		{
			if (currentSquare.File <= File.G)
			{
				var rightTakingSquare = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank - 1);

				if (boardState.GetBoardItemAt(rightTakingSquare) != null && boardState.GetBoardItemAt(rightTakingSquare).Value.Item2 != Color.BLACK
				&& !(boardState.GetBoardItemAt(rightTakingSquare).Value.Item1 == GameManager.GetPieceProfile<KingProfile>()))
				{
					takingSquares.Add(rightTakingSquare);
				}
			}
			if (currentSquare.File >= File.B)
			{
				var leftTakingSquare = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank - 1);

				if (boardState.GetBoardItemAt(leftTakingSquare) != null && boardState.GetBoardItemAt(leftTakingSquare).Value.Item2 != Color.BLACK
				&& !(boardState.GetBoardItemAt(leftTakingSquare).Value.Item1 == GameManager.GetPieceProfile<KingProfile>()))
				{
					takingSquares.Add(leftTakingSquare);
				}
			}
		}

		return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), takingSquares);
	}

	public override List<SquareCoordinate> GetCheckingSquares(BoardState boardState, SquareCoordinate currentSquare)
	{
		List<SquareCoordinate> checkingSquares = new List<SquareCoordinate>();
		Color? color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

		if (color == Color.WHITE && currentSquare.Rank <= Rank.SEVEN)
		{
			if (currentSquare.File <= File.G)
			{
				var rightCheckingSquare = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank + 1);
				checkingSquares.Add(rightCheckingSquare);
			}
			if (currentSquare.File >= File.B)
			{
				var leftCheckingSquare = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank + 1);
				checkingSquares.Add(leftCheckingSquare);

			}
		}
		else if (color == Color.BLACK && currentSquare.Rank >= Rank.TWO)
		{
			if (currentSquare.File <= File.G)
			{
				var rightCheckingSquare = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank - 1);
				checkingSquares.Add(rightCheckingSquare);

			}
			if (currentSquare.File >= File.B)
			{
				var leftCheckingSquare = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank - 1);
				checkingSquares.Add(leftCheckingSquare);
			}
		}

		return checkingSquares;
	}

	public override string ToString()
	{
		return "Pawn";
	}
}
