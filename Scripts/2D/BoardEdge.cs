using System.Linq;
using Godot;

public partial class BoardEdge : Area2D
{
	[Signal]
	public delegate void OpenPortalEventHandler(EdgeCoordinate edgeCoordinate);
	[Signal]
	public delegate void PlaceWallEventHandler(EdgeCoordinate edgeCoordinate);
	[Export]
	public Board board;
	[Export]
	public File file;
	[Export]
	public Rank rank;
	[Export]
	public Direction direction;
	private EdgeCoordinate coordinate;
	private EdgeGameData edgeGameData;

	public override void _Ready()
	{
		board.SetEdge(this);
		coordinate = GameManager.GetEdgeCoordinate(file, rank, direction);
		edgeGameData = new EdgeGameData();

		edgeGameData.IsHoveredOn = false;
		edgeGameData.OccupyingItem = null;
		Visible = false;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("click") && edgeGameData.IsHoveredOn)
		{
			OnClick();
		}
	}

	public void OnClick()
	{
		if (GameManager.GameManagerInstance.CurrentActiveGame.PendingPlacements[1] == null && !IsOccupied())
		{
			if (GameManager.GameManagerInstance.CurrentActiveGame.PendingAction == ActionType.PORTAL_PLACEMENT
			&& (GameManager.GameManagerInstance.CurrentActiveGame.PendingPlacements[0] == null)
			|| (GameManager.GameManagerInstance.CurrentActiveGame.PendingPlacements[0] != null && !GameManager.GameManagerInstance.CurrentActiveGame.PendingPlacements[0].Equals(coordinate)))
			{
				EmitSignal(SignalName.OpenPortal, coordinate);
			}
			else if (GameManager.GameManagerInstance.CurrentActiveGame.PendingAction == ActionType.WALL_PLACEMENT)
			{
				EmitSignal(SignalName.PlaceWall, coordinate);
			}
		}
	}

	public EdgeCoordinate GetCoordinate()
	{
		return coordinate;
	}

	public void SetOccupyingItem(BoardEdgeItem itemProfile)
	{
		edgeGameData.OccupyingItem = itemProfile;
	}

	public BoardEdgeItem GetOccupyingItem()
	{
		return edgeGameData.OccupyingItem;
	}

	public bool IsOccupied()
	{
		return edgeGameData.OccupyingItem != null;
	}

	public void OnMouseEntered()
	{
		edgeGameData.IsHoveredOn = true;
	}

	public void OnMouseExited()
	{
		edgeGameData.IsHoveredOn = false;
	}
}
