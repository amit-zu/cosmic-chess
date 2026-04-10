using Godot;
using System;
using static GameManager;
using static Godot.Control;

public partial class PromotionMenu : Node2D
{
	[Export]
	private Color color;
	private Sprite2D selectedPieceSprite;
	private Pawn promotedPawn;
	private Camera2D camera;
	private PromotionManager promotionManager;

	public override void _Ready()
	{
		selectedPieceSprite = new Sprite2D
		{
			Scale = new Vector2(0.4F, 0.5F),
			Position = new Vector2(0F, -2F),
			Visible = true,
			ZIndex = 5
		};

		selectedPieceSprite.Texture = BoardItemInfo.GetSprite(promotionManager.GetPieceTypesList().GetHeadOfList(), color);

		AddChild(selectedPieceSprite);

		camera = GetViewport().GetCamera2D();
	}

	public override void _Process(double delta)
	{
		if (camera != null && Rotation != camera.Rotation)
		{
			Rotation = camera.Rotation;

			Position = promotedPawn.Position + new Vector2(0, Rotation == 0 ? -25 : 25);
		}
	}

	public void SetPromotionManager(PromotionManager promotionManager)
	{
		this.promotionManager = promotionManager;
	}

	public Color GetColor()
	{
		return color;
	}

	public void SetColor(Color color)
	{
		this.color = color;
	}

	public void NextPiece()
	{
		promotionManager.NextPiece();
	}

	public void PreviouspPiece()
	{
		promotionManager.PreviouspPiece();
	}

	public void SetTexture(Texture2D texture)
	{
		selectedPieceSprite.Texture = texture;
	}

	public void SetPromotedPawn(Pawn pawn)
	{
		promotedPawn = pawn;
	}

	public void Promote(Node viewport, InputEvent inputeEvent, int shapdeIdx)
	{
		if (inputeEvent is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				promotionManager.PromotePawn(promotedPawn.GetSquare().GetCoordinate(), color);
			}
		}
	}

	public void Enable()
	{
		Visible = true;

		Button leftButton = GetNode<Button>("TextureRect/LeftButton");
		Button rightButton = GetNode<Button>("TextureRect/RightButton");
		CollisionShape2D choosePieceButtonCollisionShape = GetNode<CollisionShape2D>("TextureRect/ChoosePieceArea/CollisionShape2D");

		leftButton.Visible = true;
		rightButton.Visible = true;

		leftButton.MouseFilter = MouseFilterEnum.Stop;
		rightButton.MouseFilter = MouseFilterEnum.Stop;

		choosePieceButtonCollisionShape.Disabled = false;
	}

	public void Disable()
	{
		Visible = false;

		Button leftButton = GetNode<Button>("TextureRect/LeftButton");
		Button rightButton = GetNode<Button>("TextureRect/RightButton");
		CollisionShape2D choosePieceButtonCollisionShape = GetNode<CollisionShape2D>("TextureRect/ChoosePieceArea/CollisionShape2D");

		leftButton.Visible = false;
		rightButton.Visible = false;

		leftButton.MouseFilter = MouseFilterEnum.Ignore;
		rightButton.MouseFilter = MouseFilterEnum.Ignore;

		choosePieceButtonCollisionShape.Disabled = true;
	}
}
