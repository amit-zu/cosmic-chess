using Godot;
using System;
using System.Collections.Generic;
using static GameManager;

[GlobalClass]
public partial class GameScreen : Control
{
	[Export]
	protected Game Game;
	[Export]
	protected Color PlayerColor;
	[Export]
	protected Camera2D Camera2d;
	[Export]
	protected TextureRect Background;
	[Export]
	protected PromotionManager PromotionManager;
	[Export]
	protected MoveManager MoveManager;
	[Export]
	protected BotManager BotManager;

	public override void _Ready()
	{
		Init2DBoard();

		Game.GameEnd += ShowGameEndPopup;

		Game.PlayerColor = PlayerColor;

		GetViewport().SizeChanged += UpdateCameraZoom;

		MoveManager.PlacementEnded += EndEdgeItemSelection;

		UpdateCameraZoom();
	}

	public void FlipCamera()
	{
		Camera2d.Rotation = Camera2d.Rotation == (float)Math.PI ? 0 : (float)Math.PI;
		Background.Position = new Vector2(-Background.Position.X, -Background.Position.Y);
		Rotate2DPieces();
	}

	public void UpdateCameraZoom()
	{
		Vector2 standardSize = new Vector2(1920, 991);
		Vector2 standardCameraToViewportRatio = new Vector2(5, 5) / standardSize;

		Camera2d.Zoom = standardCameraToViewportRatio * GetViewport().GetVisibleRect().Size;
	}

	public void ShowGameEndPopup(int gameConclusionType, int winnerColor)
	{
		CanvasLayer gameEndPopup = GetNode<CanvasLayer>("GameEndPopup");
		Label whoWonLabel = gameEndPopup.GetNode<Label>("Control/Panel/WhoWonLabel");
		Label conclusionTypeLabel = gameEndPopup.GetNode<Label>("Control/Panel/ConclusionTypeLabel");

		if ((GameConclusionType)gameConclusionType == GameConclusionType.STALEMATE)
		{
			whoWonLabel.Text = "Draw";
			conclusionTypeLabel.Text = "By Stalemate";
		}
		else if ((GameConclusionType)gameConclusionType == GameConclusionType.CHECKMATE)
		{
			whoWonLabel.Text = (winnerColor == (int)Color.WHITE ? "White" : "Black") + " Wins";
			conclusionTypeLabel.Text = "By Checkmate";
		}
		else if ((GameConclusionType)gameConclusionType == GameConclusionType.TIMEOUT)
		{
			whoWonLabel.Text = (winnerColor == (int)Color.WHITE ? "White" : "Black") + " Wins";
			conclusionTypeLabel.Text = "By Timeout";
		}
		else if ((GameConclusionType)gameConclusionType == GameConclusionType.LEVEL_CLEARED)
		{
			whoWonLabel.Text = "Level Cleared";
			conclusionTypeLabel.Text = "Congratulations!";
		}

		Timer timer = new Timer();
		timer.WaitTime = 0.5;
		timer.OneShot = true;
		timer.Timeout += () =>
		{
			gameEndPopup.Visible = true;
		};
		gameEndPopup.AddChild(timer);
		timer.Start();
	}

	public void OnSpecialPowerIconClick(Type type)
	{
		if (type.IsSubclassOf(typeof(SpecialPowerIcon)))
		{
			if (type == typeof(PortalIcon))
			{
				BeginPortalSelection();
			}
			else if (type == typeof(WallIcon))
			{
				BeginWallPlacement();
			}

			DisableAllIcons();
		}
		else
		{
			// throw error
		}
	}

	public void DisableAllIcons()
	{
		foreach (Node child in GetNode<Control>("UICanvasLayer/UI/RightPanel/TabContainer/Special/SpecialItemsContainer").GetChildren())
		{
			if (child is SpecialPowerIcon icon)
			{
				icon.Disable();
			}
		}
	}

	public void EnableAllIcons()
	{
		foreach (Node child in GetNode<Control>("UICanvasLayer/UI/RightPanel/TabContainer/Special/SpecialItemsContainer").GetChildren())
		{
			if (child is SpecialPowerIcon icon)
			{
				icon.Enable();
			}
		}
	}

	public void BeginPortalSelection()
	{
		Game.PendingPlacements = [null, null];
		Game.PendingAction = ActionType.PORTAL_PLACEMENT;

		DisplayMessage("Select a portal location.");
		Board board = GetNode<Board>("2D/Board2D");

		board.ShowAllAvailableEdges();
	}

	public void EndEdgeItemSelection()
	{
		Game.PendingPlacements = [null, null];
		Board board = GetNode<Board>("2D/Board2D");
		board.HideAllEdges();
		Game.PendingAction = ActionType.NONE;
		CloseMessage();
		EnableAllIcons();
	}

	public void BeginWallPlacement()
	{
		Game.PendingAction = ActionType.WALL_PLACEMENT;
		DisplayMessage("Select a wall location.");
		Board board = GetNode<Board>("2D/Board2D");

		board.ShowAllAvailableEdges();
	}

	public void UpdateSideUI()
	{
		ClearSideUI();
		FillSideUI();
	}

	public Panel DisplayMessage(string message)
	{
		Control UIControl = GetNode<Control>("UICanvasLayer/UI");
		Panel UIMessage = GD.Load<PackedScene>("res://Scenes/UI/ui_message.tscn").Instantiate<Panel>();
		Label messageLabel = UIMessage.GetNode<Label>("MessageLabel");
		messageLabel.Text = message;
		UIControl.AddChild(UIMessage);
		return UIMessage;
	}

	public void ChangeActionAvailability()
	{
		if (Game.CurrentTurn == PlayerColor && Game.GameType != GameType.SelfPlay && Game.PendingAction == ActionType.NONE)
		{
			EnableGameInteraction();
		}
		else
		{
			DisableGameInteraction();
		}
	}

	public void DisableGameInteraction()
	{
		DisableAllIcons();
		DisableAllPieces();
	}

	public void EnableGameInteraction()
	{
		EnableAllIcons();
		EnableAllPieces();
	}

	public void PauseClocks()
	{
		Game.PlayerWhite.StopClock();
		Game.PlayerBlack.StopClock();
	}

	public void ResumeClocks()
	{
		Game.PlayerWhite.StartClock();
		Game.PlayerBlack.StartClock();
	}

	public void CloseMessage()
	{
		Panel messagePanel = GetNode<Panel>("UICanvasLayer/UI/UIMessage");
		if (messagePanel != null && !messagePanel.IsQueuedForDeletion())
		{
			messagePanel.QueueFree();
		}
	}

	public MoveManager GetMoveManager()
	{
		return MoveManager;
	}

	private void ClearSideUI()
	{
		Container whiteCapturedPiecesContainer = GetNode<Container>("UICanvasLayer/UI/RightPanel/TabContainer/Status/WhiteCapturedPieces");
		Container blackCapturedPiecesContainer = GetNode<Container>("UICanvasLayer/UI/RightPanel/TabContainer/Status/BlackCapturedPieces");

		foreach (Node node in whiteCapturedPiecesContainer.GetChildren())
		{
			node.QueueFree();
		}

		foreach (Node node in blackCapturedPiecesContainer.GetChildren())
		{
			node.QueueFree();
		}
	}

	private void FillSideUI()
	{
		string miniPieceScenePath = "res://Scenes/2D/Pieces/mini_piece.tscn";

		Container whiteCapturedPiecesContainer = GetNodeOrNull<Container>("UICanvasLayer/UI/RightPanel/TabContainer/Status/WhiteCapturedPieces");
		Container blackCapturedPiecesContainer = GetNodeOrNull<Container>("UICanvasLayer/UI/RightPanel/TabContainer/Status/BlackCapturedPieces");

		Label whiteTotalPointsLabel = GetNodeOrNull<Label>("UICanvasLayer/UI/RightPanel/TabContainer/Status/WhiteTotalPointsLabel");
		Label blackTotalPointsLabel = GetNodeOrNull<Label>("UICanvasLayer/UI/RightPanel/TabContainer/Status/BlackTotalPointsLabel");

		if (whiteCapturedPiecesContainer == null || blackCapturedPiecesContainer == null ||
			whiteTotalPointsLabel == null || blackTotalPointsLabel == null)
		{
			return;
		}

		int whiteTotalPoints = 0;
		int blackTotalPoints = 0;

		Dictionary<Type, int> whiteCaptures = new Dictionary<Type, int>();
		Dictionary<Type, int> blackCaptures = new Dictionary<Type, int>();

		foreach (PieceProfile piece in GameManagerInstance.CurrentActiveGame.PlayerWhite.CapturedPieces)
		{
			whiteTotalPoints += piece.Points;

			Type pieceType = piece.GetType();
			if (whiteCaptures.ContainsKey(pieceType))
				whiteCaptures[pieceType]++;

			else
				whiteCaptures[pieceType] = 1;
		}

		foreach (PieceProfile piece in GameManagerInstance.CurrentActiveGame.PlayerBlack.CapturedPieces)
		{
			blackTotalPoints += piece.Points;

			Type pieceType = piece.GetType();
			if (blackCaptures.ContainsKey(pieceType))
				blackCaptures[pieceType]++;

			else
				blackCaptures[pieceType] = 1;
		}

		Type[] orderedPieceTypes = {
			typeof(PawnProfile),
			typeof(KnightProfile),
			typeof(BishopProfile),
			typeof(ArchbishopProfile),
			typeof(RookProfile),
			typeof(QueenProfile),
			typeof(KingProfile)
		};

		int position = -40;
		int spacing = 40;

		foreach (Type pieceType in orderedPieceTypes)
		{
			if (!whiteCaptures.ContainsKey(pieceType))
				continue;

			int captureCount = whiteCaptures[pieceType];

			HBoxContainer itemContainer = new HBoxContainer();
			itemContainer.AnchorLeft = 0.5f;
			itemContainer.AnchorRight = 0.5f;
			itemContainer.OffsetTop = position + spacing;
			itemContainer.GrowHorizontal = Control.GrowDirection.Both;
			itemContainer.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;

			PieceProfile pieceProfile = GetPieceProfileByType(pieceType);
			var miniPiece = (MiniPiece)GD.Load<PackedScene>(miniPieceScenePath).Instantiate();
			miniPiece.SetPiece(pieceProfile, Color.BLACK);

			Label countLabel = new Label();
			countLabel.Text = "x" + captureCount.ToString();
			countLabel.HorizontalAlignment = HorizontalAlignment.Left;
			countLabel.VerticalAlignment = VerticalAlignment.Center;

			itemContainer.AddChild(miniPiece);
			itemContainer.AddChild(countLabel);
			whiteCapturedPiecesContainer.AddChild(itemContainer);

			position += spacing;
		}

		position = -40;

		foreach (Type pieceType in orderedPieceTypes)
		{
			if (!blackCaptures.ContainsKey(pieceType))
				continue;

			int captureCount = blackCaptures[pieceType];

			HBoxContainer itemContainer = new HBoxContainer();
			itemContainer.AnchorLeft = 0.5f;
			itemContainer.AnchorRight = 0.5f;
			itemContainer.OffsetTop = position + spacing;
			itemContainer.GrowHorizontal = Control.GrowDirection.Both;
			itemContainer.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;

			PieceProfile pieceProfile = GetPieceProfileByType(pieceType);
			var miniPiece = (MiniPiece)GD.Load<PackedScene>(miniPieceScenePath).Instantiate();
			miniPiece.SetPiece(pieceProfile, Color.WHITE);

			Label countLabel = new Label();
			countLabel.Text = "x" + captureCount.ToString();
			countLabel.HorizontalAlignment = HorizontalAlignment.Left;
			countLabel.VerticalAlignment = VerticalAlignment.Center;

			itemContainer.AddChild(miniPiece);
			itemContainer.AddChild(countLabel);
			blackCapturedPiecesContainer.AddChild(itemContainer);

			position += spacing;
		}

		whiteTotalPointsLabel.Text = "Points: " + whiteTotalPoints.ToString();
		blackTotalPointsLabel.Text = "Points: " + blackTotalPoints.ToString();
	}

	private void Rotate2DPieces()
	{
		Board board = GetNode<Board>("2D/Board2D");

		for (File file = File.A; file <= File.H; file++)
		{
			for (Rank rank = Rank.ONE; rank <= Rank.EIGHT; rank++)
			{
				Piece piece = board.GetPieceOccupyingSquareOrNull(GetSquareCoordinate(file, rank));
				if (piece != null)
				{
					piece.Rotate((float)Math.PI);
				}
			}
		}
	}

	private bool AreNodesStillInitializing(Node rootNode)
	{
		foreach (Node child in rootNode.GetChildren())
		{
			if (!child.IsInsideTree())
			{
				return true;
			}

			if (AreNodesStillInitializing(child))
			{
				return true;
			}
		}

		return false;
	}

	private void Init2DBoard()
	{
		BoardState boardState = GameManagerInstance.CurrentActiveGame.GetLastBoardState();
		PromotionManager promotionManager = GetNode<PromotionManager>("PromotionManager");
		Board board = MoveManager.GetBoard();

		foreach (BoardEdge edge in board.GetEdges())
		{
			edge.OpenPortal += MoveManager.OpenPortal;
			edge.PlaceWall += MoveManager.PlaceWall;
		}

		for (Rank rank = Rank.ONE; rank <= Rank.EIGHT; rank++)
		{
			for (File file = File.A; file <= File.H; file++)
			{
				(PieceProfile, Color)? boardItem = boardState.GetBoardItemAt(GetSquareCoordinate(file, rank));

				if (boardItem.HasValue)
				{
					Init2DPiece(board.GetSquareByFileAndRank(GetSquareCoordinate(file, rank)), boardItem.Value.Item1, boardItem.Value.Item2, promotionManager);
				}
			}
		}
	}

	private void Init2DPiece(Square square, PieceProfile pieceProfile, Color color, PromotionManager promotionManager)
	{
		Node piecesNode = GetNode("2D/Pieces");
		Board board = GetNode<Board>("2D/Board2D");

		string pieceScenePath = BoardItemInfo.Get2DScenePath(pieceProfile);
		var piece = (Piece)GD.Load<PackedScene>(pieceScenePath).Instantiate();
		piece.SetBoard(board);
		piece.SetSquare(square);
		piece.SetColor(color);
		piece.Promotion += MoveManager.StartPromotion;
		piece.MovePiece += MoveManager.MovePiece;
		piece.CapturePiece += MoveManager.CapturePiece;
		piece.Castle += MoveManager.Castle;
		piece.ZIndex = 8 - (int)square.GetRank();

		piecesNode.AddChild(piece);
	}

	private void EnableAllPieces()
	{
		foreach (Node child in GetNode<Node>("2D/Pieces").GetChildren())
		{
			if (child is Piece piece)
			{
				piece.Enable();
			}
		}
	}

	private void DisableAllPieces()
	{
		foreach (Node child in GetNode<Node>("2D/Pieces").GetChildren())
		{
			if (child is Piece piece)
			{
				piece.Disable();
			}
		}
	}
}
