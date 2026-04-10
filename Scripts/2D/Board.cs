using Godot;

public partial class Board : Node2D
{
	private Square[,] Squares;
	private BoardEdge[,] Edges;

	public override void _EnterTree()
	{
		Squares = new Square[8, 8];
		Edges = new BoardEdge[4, 8];
	}

	public void SetSquare(Square square)
	{
		Squares[(int)square.GetFile() - 1, (int)square.GetRank() - 1] = square;
	}

	public Square[,] GetSquares()
	{
		return Squares;
	}

	public BoardEdge[,] GetEdges()
	{
		return Edges;
	}

	public void SetEdge(BoardEdge edge)
	{
		switch (edge.direction)
		{
			case Direction.Up:
				Edges[0, (int)edge.file - 1] = edge;
				break;
			case Direction.Down:
				Edges[1, (int)edge.file - 1] = edge;
				break;
			case Direction.Left:
				Edges[2, (int)edge.rank - 1] = edge;
				break;
			case Direction.Right:
				Edges[3, (int)edge.rank - 1] = edge;
				break;
		}
	}

	public Square GetSquareByFileAndRank(SquareCoordinate coordinate)
	{
		return Squares[(int)coordinate.File - 1, (int)coordinate.Rank - 1];
	}

	public Piece GetPieceOccupyingSquareOrNull(SquareCoordinate square)
	{
		return Squares[(int)square.File - 1, (int)square.Rank - 1].GetOccupyingPiece();
	}

	public BoardEdge GetEdgeByCoordinate(EdgeCoordinate coordinate)
	{
		foreach (BoardEdge edge in Edges)
		{
			if (edge != null && edge.GetCoordinate().Equals(coordinate))
			{
				return edge;
			}
		}

		return null;
	}

	public void MarkLegalSquares(SquareCoordinate[] squares)
	{
		foreach (SquareCoordinate coordinate in squares)
		{
			Square square = GetSquareByFileAndRank(coordinate);
			square.Visible = true;
			square.SetIsTakingSquare(false);
		}
	}

	public void MarkEnPassantSquare(SquareCoordinate enPassantSquare, SquareCoordinate initialSquare, Color color)
	{
		Square square = GetSquareByFileAndRank(enPassantSquare);
		square.Visible = true;
		square.SetEnPassantDirection(initialSquare, color);
	}

	public void MarkTakingSquares(SquareCoordinate[] squares)
	{
		foreach (SquareCoordinate coordinate in squares)
		{
			Square square = GetSquareByFileAndRank(coordinate);
			square.Visible = true;
			square.SetIsTakingSquare(true);
		}
	}

	public void MarkCastlingSquares(SquareCoordinate[] squares)
	{
		foreach (SquareCoordinate coordinate in squares)
		{
			Square square = GetSquareByFileAndRank(coordinate);
			square.Visible = true;
			square.SetIsCastlingSquare(true);
		}
	}

	public void UnmarkAllSquares()
	{
		foreach (Square square in Squares)
		{
			square.Visible = false;
			square.SetIsTakingSquare(false);
			square.SetIsCastlingSquare(false);
			square.SetEnPassantDirection(null, null);
		}
	}

	public void ShowAllAvailableEdges()
	{
		foreach (BoardEdge edge in Edges)
		{
			if (edge != null && edge.GetOccupyingItem() == null ||
			(GameManager.GameManagerInstance.CurrentActiveGame.PendingAction == ActionType.WALL_PLACEMENT) && edge.GetOccupyingItem() is Portal)
			{
				edge.Visible = true;
			}
		}
	}

	public void HideAllEdges()
	{
		foreach (BoardEdge edge in Edges)
		{
			if (edge != null)
			{
				edge.Visible = false;
			}
		}
	}
}
