using Godot;
using System;

public partial class Background : TextureRect
{
	[Export]
	private Camera2D camera2D;

	public override void _Ready()
	{
		if (camera2D == null)
		{
			GD.PrintErr("Camera2D is not assigned in the inspector.");
			return;
		}

		AlignBackgroundWithCamera();
	}

	public override void _Process(double delta)
	{
		AlignBackgroundWithCamera();
	}

	private void AlignBackgroundWithCamera()
	{
		if (camera2D == null)
			return;

		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		Vector2 worldSize = screenSize / camera2D.Zoom;
		Size = worldSize;

		Rotation = camera2D.Rotation;

		if (camera2D.Rotation == 0)
		{
			Position = camera2D.GlobalPosition - (worldSize / 2.0f);
		}
	}
}
