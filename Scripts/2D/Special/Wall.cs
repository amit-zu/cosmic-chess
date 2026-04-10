using Godot;
using System;

public partial class Wall : BoardEdgeItem
{
	private Sprite2D sprite2D;

	public override void _Ready()
	{
		sprite2D = GetNode<Sprite2D>("Sprite2D");
	}

	public void SetFrontTexture()
	{
		sprite2D.Texture = GD.Load<Texture2D>("res://Assets/2D/Special Sprites/Walls/brick_wall_front.png");
		sprite2D.Scale = new Vector2(0.4f, 0.4f);
	}

	public void SetSideTexture()
	{
		sprite2D.Texture = GD.Load<Texture2D>("res://Assets/2D/Special Sprites/Walls/brick_wall_side.png");
		sprite2D.Scale = new Vector2(0.02f, 0.02f);
	}
}
