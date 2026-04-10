using Godot;
using System;

public partial class MiniPiece : Control
{
	public PieceProfile PieceProfile;
	public Color Color;
	public Sprite2D PieceSprite;

	public override void _Ready()
	{
		base._Ready();

		PieceSprite = GetNode<Sprite2D>("Sprite2D");
		PieceSprite.Texture = MiniChessPieceSprites.LoadSprite(PieceProfile, Color);
		PieceSprite.Scale = new Vector2(2, 2);
	}

	public void SetPiece(PieceProfile pieceProfile, Color color)
	{
		PieceProfile = pieceProfile;
		Color = color;
	}
}
