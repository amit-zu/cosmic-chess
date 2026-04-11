using Godot;
using static GameManager;

public partial class MoveManager : Node
{
	[Export]
	public Board Board;
	[Export]
	public Game Game;
	[Signal]
	public delegate void PlacementEndedEventHandler();

	public Board GetBoard()
	{
		return Board;
	}

	public void MakeMove(Move move)
	{
		BoardState currentBoardState = Game.GetLastBoardState();

		if (move.CastlingType != null)
		{
			Castle(move.OriginSquare, move.DestinationSquare, (int)move.CastlingType, (int)move.Color);
			return;
		}
		else if (move.IsEnPassant)
		{
			EnPassantContext enPassantContext = Game.EnPassantContext;
			CapturePiece(enPassantContext.CapturedPawnCoordinate);
			MovePiece(move.OriginSquare, move.DestinationSquare, move.PieceProfile, (int)move.Color);
		}
		else if (move.PromotionPieceProfile != null)
		{
			MovePiece(move.OriginSquare, move.DestinationSquare, move.PieceProfile, (int)move.Color);
			Promote(move.DestinationSquare, move.PromotionPieceProfile, move.Color);
		}
		else
		{
			if (currentBoardState.GetBoardItemAt(move.DestinationSquare) != null)
			{
				CapturePiece(move.DestinationSquare);
			}
			MovePiece(move.OriginSquare, move.DestinationSquare, move.PieceProfile, (int)move.Color);
		}
	}

	public void MovePiece(SquareCoordinate initialSquare, SquareCoordinate destinationSquare, PieceProfile pieceProfile, int color)
	{
		if (MovePieceOnBoard(initialSquare, destinationSquare))
		{
			UpdateGameStateForMove(pieceProfile, color, initialSquare, destinationSquare);
		}
	}

	// this function only removes the piece from the board and does not update the game state
	public void CapturePiece(SquareCoordinate pieceCoordinate)
	{
		Node piecesNode = Util.FindChild(GetTree().Root, "Pieces");

		Piece piece = Board.GetPieceOccupyingSquareOrNull(pieceCoordinate);

		if (piece != null)
		{
			piecesNode.RemoveChild(piece);
		}
		else
		{
			// throw error
		}
	}

	public void Castle(SquareCoordinate originSquare, SquareCoordinate destinationSquare, int castlingType, int color)
	{
		BoardState currentBoardState = Game.GetLastBoardState();

		SquareCoordinate rookOriginSquare = GetCastlingRookCoordinate((Color)color, (CastlingType)castlingType);
		SquareCoordinate rookDestinationSquare = (CastlingType)castlingType == CastlingType.KINGSIDE ?
			Game.GetRookDestinationForKingsideCastling((Color)color) : Game.GetRookDestinationForQueensideCastling((Color)color);

		MovePieceOnBoard(originSquare, destinationSquare);
		MovePieceOnBoard(rookOriginSquare, rookDestinationSquare);
		UpdateGameStateForCastling((CastlingType)castlingType, originSquare, destinationSquare, rookOriginSquare, rookDestinationSquare, color);
	}

	public void StartPromotion(SquareCoordinate pawnOriginSquare, SquareCoordinate pawnDestinationSquare, int color)
	{
		PromotionManager promotionManager = Util.FindChild<PromotionManager>(GetTree().Root);
		MovePieceOnBoard(pawnOriginSquare, pawnDestinationSquare);
		Game.UpdateBoardStateForMove(GetPieceProfile<PawnProfile>(), (Color)color, pawnOriginSquare, pawnDestinationSquare);
		promotionManager.OpenPromotionMenu(pawnDestinationSquare);
	}

	public void Promote(SquareCoordinate promotionSquare, PieceProfile pieceProfile, Color color)
	{
		InitPromotionPiece(promotionSquare, pieceProfile, color);

		UpdateGameStateForPromotion(promotionSquare, pieceProfile, (int)color);
	}

	public void OpenPortal(EdgeCoordinate edgeCoordinate)
	{
		Portal portal = GD.Load<PackedScene>("res://Scenes/2D/Special Powers/portal.tscn").Instantiate<Portal>();
		BoardEdge boardEdge = Board.GetEdgeByCoordinate(edgeCoordinate);
		portal.Position = boardEdge.GlobalPosition;
		portal.Rotation += boardEdge.Rotation;
		portal.Coordinate = edgeCoordinate;
		Util.FindChild<GameScreen>(GetTree().Root).AddChild(portal);

		if (GameManagerInstance.CurrentActiveGame.PendingPlacements[0] == null)
		{
			GameManagerInstance.CurrentActiveGame.PendingPlacements[0] = edgeCoordinate;
		}
		else if (GameManagerInstance.CurrentActiveGame.PendingPlacements[1] == null)
		{
			GameManagerInstance.CurrentActiveGame.PendingPlacements[1] = edgeCoordinate;

			UpdateGameStateForPlacement(GetBoardEdgeItemProfile<PortalProfile>(), GameManagerInstance.CurrentActiveGame.PendingPlacements[0],
			GameManagerInstance.CurrentActiveGame.PendingPlacements[1]);

			BoardEdgeItem connectedEdgeItem = Board.GetEdgeByCoordinate(GameManagerInstance.CurrentActiveGame.PendingPlacements[0]).GetOccupyingItem();

			if (connectedEdgeItem != null)
			{
				connectedEdgeItem.ConnectedEdgeItem = portal;
				portal.ConnectedEdgeItem = connectedEdgeItem;
			}

			EmitSignal(SignalName.PlacementEnded);
		}

		Board.GetEdgeByCoordinate(edgeCoordinate).SetOccupyingItem(portal);
	}

	public void PlaceWall(EdgeCoordinate edgeCoordinate)
	{
		Wall wall = GD.Load<PackedScene>("res://Scenes/2D/Special Powers/wall.tscn").Instantiate<Wall>();
		BoardEdge boardEdge = Board.GetEdgeByCoordinate(edgeCoordinate);
		wall.Position = boardEdge.GlobalPosition;
		wall.Coordinate = edgeCoordinate;
		Util.FindChild<GameScreen>(GetTree().Root).AddChild(wall);

		Direction wallDirection = Util.GetOppositeDirection(edgeCoordinate.Direction);
		if (wallDirection == Direction.Up || wallDirection == Direction.Down)
		{
			wall.SetFrontTexture();
		}
		else
		{
			wall.SetSideTexture();

			if (wallDirection == Direction.Right)
			{
				wall.Scale = new Vector2(-wall.Scale.X, wall.Scale.Y);
			}
		}

		boardEdge.SetOccupyingItem(wall);
		UpdateGameStateForPlacementOverride(GetBoardEdgeItemProfile<WallProfile>(), edgeCoordinate);
		EmitSignal(SignalName.PlacementEnded);
	}

	private void UpdateGameStateForMove(PieceProfile pieceProfile, int color, SquareCoordinate initialSquare, SquareCoordinate finalSquare)
	{
		PieceProfile? capturedPieceProfile = Board.GetPieceOccupyingSquareOrNull(finalSquare)?.GetProfile();

		Game.UpdateBoardStateForMove(pieceProfile, (Color)color, initialSquare, finalSquare);

		Game.AddMove(new Move(pieceProfile, initialSquare, finalSquare, (Color)color, capturedPieceProfile));
		Game.ChangeTurn();
	}

	private void UpdateGameStateForPlacement(BoardEdgeItemProfile itemProfile, EdgeCoordinate edgeCoordinate, EdgeCoordinate? connectedEdgeItemCoordinate)
	{
		Game.UpdateBoardStateForPlacement(itemProfile, edgeCoordinate, connectedEdgeItemCoordinate);
		Game.ChangeTurn();
	}

	private void UpdateGameStateForPlacementOverride(BoardEdgeItemProfile itemProfile, EdgeCoordinate edgeCoordinate)
	{
		Game.UpdateBoardStateForPlacementOverride(itemProfile, edgeCoordinate);
		Game.ChangeTurn();
	}

	private void UpdateGameStateForCastling(CastlingType castlingType, SquareCoordinate kingInitialSquare, SquareCoordinate kingDestinationSquare, SquareCoordinate rookInitialSquare, SquareCoordinate rookDestinationSquare, int color)
	{
		Game.UpdateBoardStateForCastling(kingInitialSquare, kingDestinationSquare, rookInitialSquare, rookDestinationSquare, (Color)color);

		Game.AddMove(new Move(GetBoard().GetPieceOccupyingSquareOrNull(kingDestinationSquare).GetProfile(), kingInitialSquare, kingDestinationSquare, (Color)color, null, null, castlingType));
		Game.ChangeTurn();
	}

	private void UpdateGameStateForPromotion(SquareCoordinate pawnSquareCoordinate, PieceProfile promotionPieceProfile, int color)
	{
		Game.UpdateBoardStateForPromotion(pawnSquareCoordinate, promotionPieceProfile, (Color)color);

		Game.AddMove(new Move(promotionPieceProfile, pawnSquareCoordinate, pawnSquareCoordinate, (Color)color, null, promotionPieceProfile));
		Game.ChangeTurn();
	}

	private bool MovePieceOnBoard(SquareCoordinate initialSquare, SquareCoordinate finalSquare)
	{
		Piece piece = Board.GetPieceOccupyingSquareOrNull(initialSquare);

		if (piece != null)
		{
			piece.SetSquare(Board.GetSquareByFileAndRank(initialSquare), Board.GetSquareByFileAndRank(finalSquare));
			return true;
		}
		else
		{
			GD.PrintErr($"Failed to move piece: No piece found at {initialSquare}");
			return false;
		}
	}

	private SquareCoordinate GetCastlingRookCoordinate(Color color, CastlingType castlingType)
	{
		return castlingType == CastlingType.KINGSIDE
			? Game.GetLastBoardState().FindKingSideRook(color)
			: Game.GetLastBoardState().FindQueenSideRook(color);
	}

	private Piece InitPromotionPiece(SquareCoordinate pawnOriginSquare, PieceProfile pieceProfile, Color color)
	{
		Node piecesNode = Util.FindChild(GetTree().Root, "Pieces");

		Piece pawn2D = Board.GetPieceOccupyingSquareOrNull(pawnOriginSquare);

		var piece = (Piece)GD.Load<PackedScene>(BoardItemInfo.Get2DScenePath(pieceProfile)).Instantiate();

		piece.SetBoard(Board);

		piece.SetSquare(pawn2D.GetSquare());

		piece.SetColor(color);

		piece.MovePiece += Util.FindChild<GameScreen>(GetTree().Root).GetMoveManager().MovePiece;
		piece.CapturePiece += Util.FindChild<GameScreen>(GetTree().Root).GetMoveManager().CapturePiece;

		pawn2D.QueueFree();

		piece.GetSquare().SetOccupyingPiece(piece);

		piecesNode.AddChild(piece);

		piece.Enable();

		return piece;
	}
}
