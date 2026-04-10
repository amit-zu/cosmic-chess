using Godot;
using static GameManager;

public partial class BoardItem : Area2D
{
	// drag and drop variables
	protected bool isDraggable = false;
	protected Square hoveredSquare;
	protected Vector2 offset;
	private int defaultZIndex;

	public override void _Ready()
	{
		MouseEntered += _OnMouseEntered;
		MouseExited += _OnMouseExited;
		BodyEntered += _OnBodyEntered;
		BodyExited += _onBodyExited;

		defaultZIndex = ZIndex;
	}

	public override void _Process(double delta)
	{
		if (isDraggable)
		{
			if (Input.IsActionJustPressed("click"))
			{
				OnClick();
				ZIndex = 100;
			}
			else if (Input.IsActionPressed("click"))
			{
				OnDrag();
			}
			else if (Input.IsActionJustReleased("click"))
			{
				OnRelease();
				ZIndex = defaultZIndex;
			}
		}
	}

	protected virtual void OnClick()
	{

	}

	protected virtual void OnDrag()
	{

	}

	protected virtual void OnRelease()
	{
		hoveredSquare?.UnmarkHoveredSquare();
	}

	public void _OnMouseEntered()
	{
		if (!IsDragging)
		{
			isDraggable = true;
			Scale = new Vector2(0.9F, 0.9F);
		}
	}

	public void _OnMouseExited()
	{
		if (!IsDragging)
		{
			isDraggable = false;
			Scale = new Vector2(1F, 1F);
		}
	}

	public void _OnBodyEntered(Node2D body)
	{
		if (body is Square square)
		{
			if (hoveredSquare != null)
			{
				hoveredSquare.UnmarkHoveredSquare();
			}
			hoveredSquare = square;

			if (IsDragging)
			{
				hoveredSquare.MarkHoveredSquare();
			}
		}
	}

	public void _onBodyExited(Node2D body)
	{
		if (body is Square square)
		{
			square.UnmarkHoveredSquare();
		}
	}
}
