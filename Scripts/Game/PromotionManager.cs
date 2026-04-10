using Godot;
using System;
using static GameManager;

public partial class PromotionManager : Node
{
	[Export]
	public MoveManager MoveManager;
	private readonly string promotionMenuScenePath = "res://Scenes/UI/promotion_menu.tscn";
	private PromotionMenu activePromotionMenu;
	private PromotedPieceLinkedList pieceTypesList;

	public override void _Ready()
	{
		base._Ready();

		pieceTypesList = new PromotedPieceLinkedList();
	}

	public void OpenPromotionMenu(SquareCoordinate pawnCoordinate)
	{
		Board board = Util.FindChild<Board>(GetTree().Root);

		Pawn pawn = (Pawn)board.GetPieceOccupyingSquareOrNull(pawnCoordinate);

		if (pawn != null)
		{
			OpenPromotionMenu(pawn);
		}
	}

	public PromotionMenu GetActivePromotionMenu()
	{
		// Return null if the menu has been queued for deletion
		if (activePromotionMenu != null && activePromotionMenu.IsQueuedForDeletion())
			return null;

		return activePromotionMenu;
	}

	private void OpenPromotionMenu(Pawn pawn)
	{
		GameManagerInstance.CurrentActiveGame.PendingAction = ActionType.PROMOTION;

		PackedScene promotionMenuScene = (PackedScene)ResourceLoader.Load(promotionMenuScenePath);

		PromotionMenu promotionMenu = (PromotionMenu)promotionMenuScene.Instantiate();
		promotionMenu.SetPromotedPawn(pawn);
		promotionMenu.SetColor(pawn.GetColor());
		promotionMenu.Position = pawn.GetSquare().Position + new Vector2(0, -25);
		promotionMenu.SetPromotionManager(this);

		AddChild(promotionMenu);

		activePromotionMenu = promotionMenu;
	}

	public PromotedPieceLinkedList GetPieceTypesList()
	{
		return pieceTypesList;
	}

	public void NextPiece()
	{
		pieceTypesList.TraverseForward(1);

		if (activePromotionMenu != null && !activePromotionMenu.IsQueuedForDeletion())
			activePromotionMenu.SetTexture(BoardItemInfo.GetSprite(pieceTypesList.GetHeadOfList(), activePromotionMenu.GetColor()));
	}

	public void PreviouspPiece()
	{
		pieceTypesList.TraverseBackward(1);

		if (activePromotionMenu != null && !activePromotionMenu.IsQueuedForDeletion())
			activePromotionMenu.SetTexture(BoardItemInfo.GetSprite(pieceTypesList.GetHeadOfList(), activePromotionMenu.GetColor()));
	}

	public void PromotePawn(SquareCoordinate pawnPromotionSquare, Color color)
	{
		PieceProfile promotionPieceProfile = pieceTypesList.GetHeadOfList();

		MoveManager.Promote(pawnPromotionSquare, promotionPieceProfile, color);

		pieceTypesList.TraverseUntil(GetPieceProfile<QueenProfile>());

		ClosePromotionMenus();
	}

	public void ClosePromotionMenus()
	{
		GameManagerInstance.CurrentActiveGame.PendingAction = ActionType.NONE;

		if (activePromotionMenu != null && !activePromotionMenu.IsQueuedForDeletion())
		{
			activePromotionMenu.QueueFree();
		}

		activePromotionMenu = null;
	}
}
