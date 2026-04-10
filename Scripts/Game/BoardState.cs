using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static GameManager;

public class BoardState
{
	public const int BoardWidth = 8;
	public const int BoardHeight = 8;
	public List<Color> WhoIsInCheck;
	public (PieceProfile, Color)[] CheckingPieces;
	public (PieceProfile, Color)?[,] Squares { get; private set; }
	public (EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)[,] BoardEdges { get; private set; }
	public List<SquareCoordinate> SquaresWhiteIsChecking;
	public List<SquareCoordinate> SquaresBlackIsChecking;
	public string PiecesFenString;


	public BoardState()
	{
		Squares = new (PieceProfile, Color)?[BoardWidth, BoardHeight];
		BoardEdges = new (EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)[4, 8];

		WhoIsInCheck = new List<Color>();

		SquaresWhiteIsChecking = new List<SquareCoordinate>();
		SquaresBlackIsChecking = new List<SquareCoordinate>();

		DetectChecks();
	}

	public BoardState(string fenString)
	{
		Squares = new (PieceProfile, Color)?[BoardWidth, BoardHeight];
		BoardEdges = new (EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)[4, 8];

		WhoIsInCheck = new List<Color>();

		SquaresWhiteIsChecking = new List<SquareCoordinate>();
		SquaresBlackIsChecking = new List<SquareCoordinate>();

		SetBoardStateFromFenString(fenString);

		DetectChecks();
	}

	public void SetBoardStateFromFenString(string boardPart)
	{
		// TODO update this to work with board edge items
		string pieceSegment = boardPart.Split(' ')[0];
		string[] rows = pieceSegment.Split('/');

		for (Rank rank = Rank.EIGHT; rank >= Rank.ONE; rank--)
		{
			File file = File.A;

			foreach (char c in rows[(int)Rank.EIGHT - (int)rank])
			{
				if (char.IsDigit(c))
				{
					int emptySquares = (int)char.GetNumericValue(c);

					for (int i = 0; i < emptySquares; i++)
					{
						if (file > File.H) return;
						SetBoardItemAt(file, rank, null);
						file++;
					}
				}
				else
				{
					PieceProfile pieceType = BoardItemInfo.GetProfileFromSymbol(Char.ToUpper(c));
					Color? pieceColor = (pieceType != null) ? (Char.IsUpper(c) ? Color.WHITE : Color.BLACK) : null;

					if (file > File.H)
					{
						return;
					}

					if (pieceType != null && pieceColor.HasValue)
					{
						SetBoardItemAt(file, rank, (pieceType, pieceColor.Value));
					}
					else
					{
						SetBoardItemAt(file, rank, null);
					}

					file++;
				}
			}
		}
	}

	public string GetFenStringFromBoardState()
	{
		string fenString = "";

		for (Rank rank = Rank.EIGHT; rank >= Rank.ONE; rank--)
		{
			int emptyCount = 0;

			for (File file = File.A; file <= File.H; file++)
			{
				var boardItem = GetBoardItemAt(file, rank);

				if (boardItem == null)
				{
					emptyCount++;
				}
				else
				{
					if (emptyCount > 0)
					{
						fenString += emptyCount.ToString();
						emptyCount = 0;
					}

					char pieceChar = BoardItemInfo.GetSymbol(boardItem.Value.Item1);
					if (boardItem.Value.Item2 == Color.BLACK)
					{
						pieceChar = char.ToLower(pieceChar);
					}

					fenString += pieceChar;
				}
			}

			if (emptyCount > 0)
			{
				fenString += emptyCount.ToString();
			}

			if (rank != Rank.ONE)
			{
				fenString += '/';
			}
		}

		return fenString;
	}

	public void SetBoardItemAt(File file, Rank rank, (PieceProfile, Color)? typeAndColor)
	{
		if (typeAndColor == null)
		{
			Squares[(int)file - 1, (int)rank - 1] = default;
			return;
		}
		else if (typeAndColor.Value.Item1 is not PieceProfile)
		{
			GD.Print("Type " + typeAndColor.Value.Item1 + " is not a board item.");
			return;
		}

		Squares[(int)file - 1, (int)rank - 1] = typeAndColor.Value;
	}

	public (PieceProfile, Color)? GetBoardItemAt(SquareCoordinate coordinate)
	{
		return Squares[(int)coordinate.File - 1, (int)coordinate.Rank - 1];
	}

	public (PieceProfile, Color)? GetBoardItemAt(File file, Rank rank)
	{
		if (file < File.A || file > File.H || rank < Rank.ONE || rank > Rank.EIGHT)
		{
			return null;
		}

		return Squares[(int)file - 1, (int)rank - 1];
	}

	public void SetBoardEdgeItemAt(BoardEdgeItemProfile itemProfile, EdgeCoordinate coordinate, EdgeCoordinate connectedCoordinate)
	{
		BoardEdges[(int)coordinate.Direction,
		coordinate.Direction == Direction.Up || coordinate.Direction == Direction.Down ? (int)coordinate.File - 1 : (int)coordinate.Rank - 1] = (coordinate, itemProfile, connectedCoordinate);
	}

	public (EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)? GetBoardEdgeItemAt(EdgeCoordinate coordinate)
	{
		foreach (var edge in BoardEdges)
		{
			if (edge.Item2 != null && edge.Item1.Equals(coordinate))
			{
				return edge;
			}
		}

		return null;
	}

	public bool BoardContainsPieceAtSquare((PieceProfile, Color?) boardItem, SquareCoordinate square)
	{
		PieceProfile pieceType = boardItem.Item1;
		Color? pieceColor = boardItem.Item2;

		if (pieceType == null || pieceColor == null)
		{
			return false;
		}

		return GetBoardItemAt(square).Value.Item1 == pieceType && GetBoardItemAt(square).Value.Item2 == pieceColor;
	}

	public BoardState DeepCopyBoardState()
	{
		BoardState copy = new BoardState();

		for (int file = 0; file < BoardWidth; file++)
		{
			for (int rank = 0; rank < BoardHeight; rank++)
			{
				copy.Squares[file, rank] = Squares[file, rank];
			}
		}

		for (int direction = 0; direction < 4; direction++)
		{
			for (int index = 0; index < 8; index++)
			{
				var edge = BoardEdges[direction, index];

				if (edge.Item1 != null)
				{
					EdgeCoordinate newCoord1 = new EdgeCoordinate(edge.Item1.File, edge.Item1.Rank, edge.Item1.Direction);
					EdgeCoordinate newCoord2 = edge.Item3 != null ? new EdgeCoordinate(edge.Item3.File, edge.Item3.Rank, edge.Item3.Direction) : null;

					copy.BoardEdges[direction, index] = (newCoord1, edge.Item2, newCoord2);
				}
			}
		}

		copy.WhoIsInCheck = new List<Color>(WhoIsInCheck);

		copy.CheckingPieces = CheckingPieces != null ?
			CheckingPieces.Select(piece => (piece.Item1, piece.Item2)).ToArray() : null;

		copy.SquaresWhiteIsChecking = SquaresWhiteIsChecking
			.Select(sq => new SquareCoordinate(sq.File, sq.Rank))
			.ToList();

		copy.SquaresBlackIsChecking = SquaresBlackIsChecking
			.Select(sq => new SquareCoordinate(sq.File, sq.Rank))
			.ToList();

		return copy;
	}

	public BoardState GetBoardStateAfterMove((PieceProfile, Color) boardItem, SquareCoordinate initialSquare, SquareCoordinate moveSquare)
	{
		BoardState hypotheticalBoardState = DeepCopyBoardState();

		hypotheticalBoardState.SetBoardItemAt(initialSquare.File, initialSquare.Rank, null); // Remove the piece from its initial square
		hypotheticalBoardState.SetBoardItemAt(moveSquare.File, moveSquare.Rank, boardItem); // Place the piece at the new square
		hypotheticalBoardState.DetectChecks();

		return hypotheticalBoardState;
	}

	public BoardState GetBoardStateAfterPlacement(BoardEdgeItemProfile itemProfile, EdgeCoordinate coordinate, EdgeCoordinate connectedCoordinate)
	{
		BoardState hypotheticalBoardState = DeepCopyBoardState();

		hypotheticalBoardState.SetBoardEdgeItemAt(itemProfile, coordinate, connectedCoordinate);
		hypotheticalBoardState.DetectChecks();

		return hypotheticalBoardState;
	}

	public BoardState GetBoardStateAfterPlacementOverride(BoardEdgeItemProfile itemProfile, EdgeCoordinate coordinate)
	{
		BoardState hypotheticalBoardState = DeepCopyBoardState();

		(EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)? edgeItemAtCoordinate = hypotheticalBoardState.GetBoardEdgeItemAt(coordinate);
		EdgeCoordinate? connectedEdgeItemCoordinate = edgeItemAtCoordinate?.Item3;

		if (connectedEdgeItemCoordinate != null)
		{
			hypotheticalBoardState.SetBoardEdgeItemAt(null, connectedEdgeItemCoordinate, null); // set the connected edge item to null
		}

		hypotheticalBoardState.SetBoardEdgeItemAt(itemProfile, coordinate, null);

		hypotheticalBoardState.DetectChecks();

		return hypotheticalBoardState;
	}

	public BoardState GetBoardStateAfterEnPassant(SquareCoordinate capturedPawn, SquareCoordinate capturingPawnInitialSquare, SquareCoordinate destinationSquare, Color capturingPawnColor)
	{
		BoardState hypotheticalBoardState = DeepCopyBoardState();

		hypotheticalBoardState.SetBoardItemAt(capturedPawn.File, capturedPawn.Rank, null);
		hypotheticalBoardState.SetBoardItemAt(capturingPawnInitialSquare.File, capturingPawnInitialSquare.Rank, null);
		hypotheticalBoardState.SetBoardItemAt(destinationSquare.File, destinationSquare.Rank, (GameManager.GetPieceProfile<PawnProfile>(), capturingPawnColor));
		hypotheticalBoardState.DetectChecks();

		return hypotheticalBoardState;
	}

	public void PrintBoardState()
	{
		string topEdge = "  ";
		for (int file = 0; file < BoardWidth; file++)
		{
			var edge = BoardEdges[0, file];
			char edgeSymbol = '-';
			if (edge.Item2 != null)
				edgeSymbol = char.ToLower(BoardEdgeItemInfo.GetSymbol(edge.Item2));
			topEdge += edgeSymbol + " ";
		}
		GD.Print(topEdge);

		for (int rank = BoardHeight - 1; rank >= 0; rank--)
		{
			var leftEdge = BoardEdges[2, rank];
			char leftEdgeSymbol = '|';
			if (leftEdge.Item2 != null)
				leftEdgeSymbol = char.ToLower(BoardEdgeItemInfo.GetSymbol(leftEdge.Item2));

			string row = leftEdgeSymbol + " ";

			for (int file = 0; file < BoardWidth; file++)
			{
				var boardItem = Squares[file, rank];
				row += (boardItem == null ? '_' : BoardItemInfo.GetSymbol(boardItem.Value.Item1)) + " ";
			}

			var rightEdge = BoardEdges[3, rank];
			char rightEdgeSymbol = '|';
			if (rightEdge.Item2 != null)
				rightEdgeSymbol = char.ToLower(BoardEdgeItemInfo.GetSymbol(rightEdge.Item2));

			row += rightEdgeSymbol;

			GD.Print(row);
		}

		string bottomEdge = "  ";
		for (int file = 0; file < BoardWidth; file++)
		{
			var edge = BoardEdges[1, file];
			char edgeSymbol = '-';
			if (edge.Item2 != null)
				edgeSymbol = char.ToLower(BoardEdgeItemInfo.GetSymbol(edge.Item2));
			bottomEdge += edgeSymbol + " ";
		}
		GD.Print(bottomEdge);

		GD.Print();
	}

	public void DetectChecks()
	{
		List<(PieceProfile, Color?)> checkingPieces = new List<(PieceProfile, Color?)>();

		SquaresWhiteIsChecking.Clear();
		SquaresBlackIsChecking.Clear();

		for (File file = File.A; file <= File.H; file++)
		{
			for (Rank rank = Rank.ONE; rank <= Rank.EIGHT; rank++)
			{
				var boardItem = GetBoardItemAt(file, rank);

				if (boardItem == null)
					continue;

				var (pieceType, pieceColor) = boardItem.Value;

				var checkingSquares = pieceType.GetCheckingSquares(this, GetSquareCoordinate(file, rank));

				if (checkingSquares.Count > 0)
				{
					checkingPieces.Add((pieceType, pieceColor));

					if (pieceColor == Color.WHITE)
					{
						SquaresWhiteIsChecking.AddRange(checkingSquares);
					}
					else if (pieceColor == Color.BLACK)
					{
						SquaresBlackIsChecking.AddRange(checkingSquares);
					}
				}
			}
		}

		WhoIsInCheck = FindKingInCheckingSquares();
	}

	public bool IsCastlingKingsidePathClear(SquareCoordinate coordinate)
	{
		Color kingColor = GetBoardItemAt(coordinate).Value.Item2;
		SquareCoordinate rookSquare = FindKingSideRook(kingColor);
		SquareCoordinate kingDestination = Game.GetKingDestinationForKingsideCastling(kingColor);

		if (rookSquare == null)
		{
			return false;
		}

		List<SquareCoordinate> squaresBetweenKingAndDestination = Util.GetSquaresBetweenIncluding(coordinate, kingDestination);
		List<SquareCoordinate> squaresBetweenKingAndRook = Util.GetSquaresBetweenIncluding(coordinate, rookSquare);

		if (squaresBetweenKingAndRook.Any(square => !square.Equals(coordinate) && !square.Equals(rookSquare) && GetBoardItemAt(square).HasValue))
		{
			return false;
		}

		if (squaresBetweenKingAndDestination.Any(square =>
			(kingColor == Color.WHITE && Util.ContainsSquare(SquaresBlackIsChecking, square)) ||
			(kingColor == Color.BLACK && Util.ContainsSquare(SquaresWhiteIsChecking, square))))
		{
			return false;
		}

		if ((kingColor == Color.WHITE && Util.ContainsSquare(SquaresBlackIsChecking, kingDestination)) ||
			(kingColor == Color.BLACK && Util.ContainsSquare(SquaresWhiteIsChecking, kingDestination)))
		{
			return false;
		}

		return true;
	}


	public bool IsCastlingQueensidePathClear(SquareCoordinate coordinate)
	{
		Color kingColor = GetBoardItemAt(coordinate).Value.Item2;
		SquareCoordinate rookSquare = FindQueenSideRook(kingColor);
		SquareCoordinate kingDestination = Game.GetKingDestinationForQueensideCastling(kingColor);

		if (rookSquare == null)
		{
			return false;
		}

		List<SquareCoordinate> squaresBetweenKingAndDestination = Util.GetSquaresBetweenIncluding(coordinate, kingDestination);
		List<SquareCoordinate> squaresBetweenKingAndRook = Util.GetSquaresBetweenIncluding(coordinate, rookSquare);

		if (squaresBetweenKingAndRook.Any(square => !square.Equals(coordinate) && !square.Equals(rookSquare) && GetBoardItemAt(square).HasValue))
		{
			return false;
		}

		if (squaresBetweenKingAndDestination.Any(square =>
			(kingColor == Color.WHITE && Util.ContainsSquare(SquaresBlackIsChecking, square)) ||
			(kingColor == Color.BLACK && Util.ContainsSquare(SquaresWhiteIsChecking, square))))
		{
			return false;
		}

		if ((kingColor == Color.WHITE && Util.ContainsSquare(SquaresBlackIsChecking, kingDestination)) ||
			(kingColor == Color.BLACK && Util.ContainsSquare(SquaresWhiteIsChecking, kingDestination)))
		{
			return false;
		}

		return true;
	}

	public SquareCoordinate FindKingSquare(Color color)
	{
		for (File file = File.A; file <= File.H; file++)
		{
			for (Rank rank = Rank.ONE; rank <= Rank.EIGHT; rank++)
			{
				var boardItem = GetBoardItemAt(file, rank);

				if (boardItem.HasValue && boardItem.Value.Item1 == GetPieceProfile<KingProfile>() && boardItem.Value.Item2 == color)
				{
					return GetSquareCoordinate(file, rank);
				}
			}
		}

		return null;
	}

	public SquareCoordinate FindKingSideRook(Color color)
	{
		Rank rookRank = color == Color.WHITE ? Rank.ONE : Rank.EIGHT;

		for (File file = File.H; file >= File.A; file--)
		{
			var boardItem = GetBoardItemAt(file, rookRank);

			if (boardItem.HasValue && boardItem.Value.Item1 == GetPieceProfile<RookProfile>() && boardItem.Value.Item2 == color)
			{
				return GetSquareCoordinate(file, rookRank);
			}
		}

		return null;
	}


	public SquareCoordinate FindQueenSideRook(Color color)
	{
		Rank rookRank = color == Color.WHITE ? Rank.ONE : Rank.EIGHT;

		for (File file = File.A; file <= File.H; file++)
		{
			var boardItem = GetBoardItemAt(file, rookRank);

			if (boardItem.HasValue && boardItem.Value.Item1 == GetPieceProfile<RookProfile>() && boardItem.Value.Item2 == color)
			{
				return GetSquareCoordinate(file, rookRank);
			}
		}

		return null;
	}

	public Color? WhoIsCheckmated()
	{
		if (WhoIsInCheck.Count == 0)
		{
			return null;
		}

		for (File file = File.A; file <= File.H; file++)
		{
			for (Rank rank = Rank.ONE; rank <= Rank.EIGHT; rank++)
			{
				var boardItem = GetBoardItemAt(file, rank);

				if (boardItem == null)
				{
					continue;

				}

				var (pieceProfile, pieceColor) = boardItem.Value;

				if (!WhoIsInCheck.Contains(pieceColor))
				{
					continue;
				}

				if (pieceProfile.GetLegalSquares(this, GetSquareCoordinate(file, rank), true).Count > 0)
				{
					return null;
				}
				if (pieceProfile.GetTakingSquares(this, GetSquareCoordinate(file, rank)).Count > 0)
				{
					return null;
				}
			}
		}

		return WhoIsInCheck[0];
	}

	public bool IsStalemate(Color currentTurnColor)
	{
		if (WhoIsInCheck.Count > 0)
		{
			return false; // If there's a check, it's not stalemate
		}

		for (File file = File.A; file <= File.H; file++)
		{
			for (Rank rank = Rank.ONE; rank <= Rank.EIGHT; rank++)
			{
				var boardItem = GetBoardItemAt(file, rank);

				if (boardItem == null)
				{
					continue;
				}

				var (pieceProfile, pieceColor) = boardItem.Value;

				if (pieceColor != currentTurnColor)
				{
					continue; // Only check pieces of the current turn color
				}

				if (pieceProfile.GetLegalSquares(this, GetSquareCoordinate(file, rank), true).Count > 0)
				{
					return false;
				}
			}
		}

		return true; // No pieces can move and no checks, so it's stalemate
	}

	private List<Color> FindKingInCheckingSquares()
	{
		List<Color> checkedColors = new List<Color>();

		bool blackKingChecked = SquaresWhiteIsChecking.Any(square =>
		{
			var boardItem = GetBoardItemAt(square);
			return boardItem.HasValue && boardItem.Value.Item1 == GameManager.GetPieceProfile<KingProfile>() && boardItem.Value.Item2 == Color.BLACK;
		});

		bool whiteKingChecked = SquaresBlackIsChecking.Any(square =>
		{
			var boardItem = GetBoardItemAt(square);
			return boardItem.HasValue && boardItem.Value.Item1 == GameManager.GetPieceProfile<KingProfile>() && boardItem.Value.Item2 == Color.WHITE;
		});

		if (blackKingChecked) checkedColors.Add(Color.BLACK);
		if (whiteKingChecked) checkedColors.Add(Color.WHITE);

		return checkedColors;
	}
}
