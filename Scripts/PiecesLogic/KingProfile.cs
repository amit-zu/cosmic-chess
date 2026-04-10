using System.Collections.Generic;

using static GameManager;

public partial class KingProfile : PieceProfile
{
	public KingProfile()
	{
		Points = 0;
	}

	public override List<SquareCoordinate> GetLegalSquares(BoardState boardState, SquareCoordinate currentSquare, bool hasMoved)
	{
		// TODO replace everything here with Util.ContainsSquare()
		List<SquareCoordinate> legalSquares = new List<SquareCoordinate>();
		Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

		bool isSquareChecked;

		if (currentSquare.Rank <= Rank.SEVEN)
		{
			var forwardSquare = GetSquareCoordinate(currentSquare.File, currentSquare.Rank + 1);
			isSquareChecked = (color == Color.WHITE && boardState.SquaresBlackIsChecking.Contains(forwardSquare)) || (color == Color.BLACK && boardState.SquaresWhiteIsChecking.Contains(forwardSquare));

			if (boardState.GetBoardItemAt(forwardSquare) == null && !isSquareChecked)
			{
				legalSquares.Add(forwardSquare);
			}

			if (currentSquare.File <= File.G)
			{
				var forwardRightSquare = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank + 1);
				isSquareChecked = (color == Color.WHITE && boardState.SquaresBlackIsChecking.Contains(forwardRightSquare)) || (color == Color.BLACK && boardState.SquaresWhiteIsChecking.Contains(forwardRightSquare));

				if (boardState.GetBoardItemAt(forwardRightSquare) == null && !isSquareChecked)
				{
					legalSquares.Add(forwardRightSquare);
				}
			}
			if (currentSquare.File >= File.B)
			{
				var forwardLeftSquare = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank + 1);
				isSquareChecked = (color == Color.WHITE && boardState.SquaresBlackIsChecking.Contains(forwardLeftSquare)) || (color == Color.BLACK && boardState.SquaresWhiteIsChecking.Contains(forwardLeftSquare));

				if (boardState.GetBoardItemAt(forwardLeftSquare) == null && !isSquareChecked)
				{
					legalSquares.Add(forwardLeftSquare);
				}
			}
		}

		if (currentSquare.Rank >= Rank.TWO)
		{
			var backwardSquare = GetSquareCoordinate(currentSquare.File, currentSquare.Rank - 1);
			isSquareChecked = (color == Color.WHITE && boardState.SquaresBlackIsChecking.Contains(backwardSquare)) || (color == Color.BLACK && boardState.SquaresWhiteIsChecking.Contains(backwardSquare));

			if (boardState.GetBoardItemAt(backwardSquare) == null && !isSquareChecked)
			{
				legalSquares.Add(backwardSquare);
			}


			if (currentSquare.File <= File.G)
			{
				var backwardRightSquare = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank - 1);
				isSquareChecked = (color == Color.WHITE && boardState.SquaresBlackIsChecking.Contains(backwardRightSquare)) || (color == Color.BLACK && boardState.SquaresWhiteIsChecking.Contains(backwardRightSquare));

				if (boardState.GetBoardItemAt(backwardRightSquare) == null && !isSquareChecked)
				{
					legalSquares.Add(backwardRightSquare);
				}
			}
			if (currentSquare.File >= File.B)
			{
				var backwardLeftSquare = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank - 1);
				isSquareChecked = (color == Color.WHITE && boardState.SquaresBlackIsChecking.Contains(backwardLeftSquare)) || (color == Color.BLACK && boardState.SquaresWhiteIsChecking.Contains(backwardLeftSquare));

				if (boardState.GetBoardItemAt(backwardLeftSquare) == null && !isSquareChecked)
				{
					legalSquares.Add(backwardLeftSquare);
				}
			}
		}

		if (currentSquare.File <= File.G)
		{
			var rightSquare = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank);
			isSquareChecked = (color == Color.WHITE && boardState.SquaresBlackIsChecking.Contains(rightSquare)) || (color == Color.BLACK && boardState.SquaresWhiteIsChecking.Contains(rightSquare));

			if (boardState.GetBoardItemAt(rightSquare) == null && !isSquareChecked)
			{
				legalSquares.Add(rightSquare);
			}
		}
		if (currentSquare.File >= File.B)
		{
			var leftSquare = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank);
			isSquareChecked = (color == Color.WHITE && boardState.SquaresBlackIsChecking.Contains(leftSquare)) || (color == Color.BLACK && boardState.SquaresWhiteIsChecking.Contains(leftSquare));


			if (boardState.GetBoardItemAt(leftSquare) == null && !isSquareChecked)
			{
				legalSquares.Add(leftSquare);
			}
		}

		return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), legalSquares);
	}

	public override List<SquareCoordinate> GetTakingSquares(BoardState boardState, SquareCoordinate currentSquare)
	{
		List<SquareCoordinate> takingSquares = new List<SquareCoordinate>();
		var boardItem = boardState.GetBoardItemAt(currentSquare);

		if (boardItem == null)
			return takingSquares;

		Color color = boardItem.Value.Item2;
		bool isSquareChecked;

		void TryAddSquare(SquareCoordinate square)
		{
			var targetBoardItem = boardState.GetBoardItemAt(square);
			if (targetBoardItem == null) return;

			isSquareChecked = (color == Color.WHITE && boardState.SquaresBlackIsChecking.Contains(square)) ||
							  (color == Color.BLACK && boardState.SquaresWhiteIsChecking.Contains(square));

			if (targetBoardItem.Value.Item2 == Util.GetOppositeColor(color) && !isSquareChecked)
			{
				takingSquares.Add(square);
			}
		}

		if (currentSquare.Rank <= Rank.SEVEN)
		{
			TryAddSquare(GetSquareCoordinate(currentSquare.File, currentSquare.Rank + 1));
			if (currentSquare.File <= File.G) TryAddSquare(GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank + 1));
			if (currentSquare.File >= File.B) TryAddSquare(GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank + 1));
		}

		if (currentSquare.Rank >= Rank.TWO)
		{
			TryAddSquare(GetSquareCoordinate(currentSquare.File, currentSquare.Rank - 1));
			if (currentSquare.File <= File.G) TryAddSquare(GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank - 1));
			if (currentSquare.File >= File.B) TryAddSquare(GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank - 1));
		}

		if (currentSquare.File <= File.G) TryAddSquare(GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank));
		if (currentSquare.File >= File.B) TryAddSquare(GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank));

		return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), takingSquares);
	}

	public override List<SquareCoordinate> GetCheckingSquares(BoardState boardState, SquareCoordinate currentSquare)
	{
		List<SquareCoordinate> checkingSquares = new List<SquareCoordinate>();

		if (currentSquare.Rank <= Rank.SEVEN)
		{
			var forwardSquare = GetSquareCoordinate(currentSquare.File, currentSquare.Rank + 1);
			checkingSquares.Add(forwardSquare);

			if (currentSquare.File <= File.G)
			{
				var forwardRightSquare = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank + 1);
				checkingSquares.Add(forwardRightSquare);
			}
			if (currentSquare.File >= File.B)
			{
				var forwardLeftSquare = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank + 1);
				checkingSquares.Add(forwardLeftSquare);
			}
		}
		if (currentSquare.Rank >= Rank.TWO)
		{
			var backwardSquare = GetSquareCoordinate(currentSquare.File, currentSquare.Rank - 1);
			checkingSquares.Add(backwardSquare);

			if (currentSquare.File <= File.G)
			{
				var backwardRightSquare = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank - 1);
				checkingSquares.Add(backwardRightSquare);

			}
			if (currentSquare.File >= File.B)
			{
				var backwardLeftSquare = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank - 1);
				checkingSquares.Add(backwardLeftSquare);
			}
		}

		if (currentSquare.File <= File.G)
		{
			var rightSquare = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank);
			checkingSquares.Add(rightSquare);
		}
		if (currentSquare.File >= File.B)
		{
			var leftSquare = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank);
			checkingSquares.Add(leftSquare);
		}

		return checkingSquares;
	}

	public List<SquareCoordinate> GetCastlingSquares(BoardState boardState, SquareCoordinate kingSquare, CastlingContext castlingContext)
	{
		List<SquareCoordinate> castlingSquares = new List<SquareCoordinate>();

		Color color = boardState.GetBoardItemAt(kingSquare).Value.Item2;

		SquareCoordinate initialKingsideRookSquare = GameManagerInstance.CurrentActiveGame.InitialBoardState.FindKingSideRook(color);
		SquareCoordinate initialQueensideRookSquare = GameManagerInstance.CurrentActiveGame.InitialBoardState.FindQueenSideRook(color);


		if (boardState.IsCastlingKingsidePathClear(kingSquare))
		{
			if (color == Color.WHITE && castlingContext.WhiteCanCastleKingside)
			{
				castlingSquares.Add(Game.GetKingDestinationForKingsideCastling(color));
			}
			else if (color == Color.BLACK && castlingContext.BlackCanCastleKingside)
			{
				castlingSquares.Add(Game.GetKingDestinationForKingsideCastling(color));
			}
		}

		if (boardState.IsCastlingQueensidePathClear(kingSquare))
		{
			if (color == Color.WHITE && castlingContext.WhiteCanCastleQueenside)
			{
				castlingSquares.Add(Game.GetKingDestinationForQueensideCastling(color));
			}
			else if (color == Color.BLACK && castlingContext.BlackCanCastleQueenside)
			{
				castlingSquares.Add(Game.GetKingDestinationForQueensideCastling(color));
			}
		}

		return castlingSquares;
	}

	public override string ToString()
	{
		return "King";
	}
}
