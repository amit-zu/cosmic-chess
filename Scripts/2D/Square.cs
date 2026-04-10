using Godot;
using static GameManager;

public partial class Square : StaticBody2D
{
	[Export]
	private Board board;
	[Export]
	private File file;
	[Export]
	private Rank rank;
	private SquareCoordinate coordinate;
	private SquareGameData squareGameData;
	private ColorRect hoverMarking;

	public override void _Ready()
	{
		Visible = false;
		ZIndex = -1;

		coordinate = GetSquareCoordinate(file, rank);
		squareGameData = new SquareGameData();

		board.SetSquare(this);

		hoverMarking = GetNode<ColorRect>("Marking");
	}

	public void MarkHoveredSquare()
	{
		hoverMarking.Visible = true;
	}

	public void UnmarkHoveredSquare()
	{
		hoverMarking.Visible = false;
	}

	public override void _Draw()
	{
		var drawPosition = (GetNode("CollisionShape2D") as CollisionShape2D).Position;
		if (squareGameData.IsTakingSquare)
		{
			DrawArc(drawPosition, 6, 0, Mathf.Tau, 64, new Godot.Color(0.5f, 0.5f, 0.5f), 1);
		}
		else if (squareGameData.IsCastlingSquare)
		{
			DrawDashedCircle(drawPosition, 6, 12, new Godot.Color(0.5f, 0.5f, 0.5f), 1);
		}
		else if (squareGameData.EnPassantDirection.HasValue)
		{
			DrawArrow((float)squareGameData.EnPassantDirection);
		}
		else
		{
			DrawCircle(drawPosition, 3, new Godot.Color(0.5f, 0.5f, 0.5f));
		}
	}

	private void DrawDashedCircle(Vector2 center, float radius, int dashCount, Godot.Color color, float thickness)
	{
		float angleStep = Mathf.Tau / (dashCount * 2);
		for (int i = 0; i < dashCount; i++)
		{
			float startAngle = i * 2 * angleStep;
			float endAngle = startAngle + angleStep;

			Vector2 start = center + new Vector2(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * radius;
			Vector2 end = center + new Vector2(Mathf.Cos(endAngle), Mathf.Sin(endAngle)) * radius;

			DrawLine(start, end, color, thickness);
		}
	}

	private void DrawArrow(float angleDegrees)
	{
		float arrowSize = 4f;
		float length = 10f;
		float lineWidth = 1.0f;
		Godot.Color color = new Godot.Color(0.5f, 0.5f, 0.5f);

		float angleRadians = Mathf.DegToRad(angleDegrees);

		// Offset the start position so the arrow is centered
		Vector2 start = new Vector2(
			-Mathf.Cos(angleRadians) * (length / 2),
			-Mathf.Sin(angleRadians) * (length / 2)
		);

		// Snap to whole pixels for clarity
		Vector2 end = new Vector2(
			Mathf.Floor(start.X + Mathf.Cos(angleRadians) * length + 0.5f),
			Mathf.Floor(start.Y + Mathf.Sin(angleRadians) * length + 0.5f)
		);

		DrawMultiline(new Vector2[] { start, end }, color, lineWidth);

		Vector2 direction = (end - start).Normalized();

		Vector2 leftWing = new Vector2(
			Mathf.Floor(end.X - (direction.X * arrowSize) + (-direction.Y * (arrowSize / 2)) + 0.5f),
			Mathf.Floor(end.Y - (direction.Y * arrowSize) + (direction.X * (arrowSize / 2)) + 0.5f)
		);

		Vector2 rightWing = new Vector2(
			Mathf.Floor(end.X - (direction.X * arrowSize) - (-direction.Y * (arrowSize / 2)) + 0.5f),
			Mathf.Floor(end.Y - (direction.Y * arrowSize) - (direction.X * (arrowSize / 2)) + 0.5f)
		);

		DrawMultiline(new Vector2[] { end, leftWing }, color, lineWidth);
		DrawMultiline(new Vector2[] { end, rightWing }, color, lineWidth);
	}

	public File GetFile()
	{
		return coordinate.File;
	}

	public Rank GetRank()
	{
		return coordinate.Rank;
	}

	public SquareCoordinate GetCoordinate()
	{
		return coordinate;
	}

	public bool IsOccupied()
	{
		return squareGameData.IsOccupied;
	}

	public void SetIsOccupied(bool isOccupied)
	{
		squareGameData.IsOccupied = isOccupied;
	}

	public Piece GetOccupyingPiece()
	{
		return squareGameData.Occupying2DPiece;
	}

	public void SetOccupyingPiece(Piece piece)
	{
		squareGameData.Occupying2DPiece = piece;

		if (piece != null)
		{
			SetIsOccupied(true);
		}
	}

	public void SetIsTakingSquare(bool value)
	{
		squareGameData.IsTakingSquare = value;
	}

	public void ClearOccupyingPiece()
	{
		SetOccupyingPiece(null);
		SetIsOccupied(false);
	}

	public void SetIsCastlingSquare(bool value)
	{
		squareGameData.IsCastlingSquare = value;
	}

	public void SetEnPassantDirection(SquareCoordinate initialSquare, Color? pieceColor)
	{
		if (initialSquare == null)
		{
			squareGameData.EnPassantDirection = null;
			return;
		}

		if (pieceColor == Color.WHITE)
		{
			if (file < initialSquare.File)
			{
				squareGameData.EnPassantDirection = EnPassantDirection.TopLeft;
			}
			else
			{
				squareGameData.EnPassantDirection = EnPassantDirection.TopRight;
			}
		}
		else if (pieceColor == Color.BLACK)
		{
			if (file < initialSquare.File)
			{
				squareGameData.EnPassantDirection = EnPassantDirection.BottomLeft;
			}
			else
			{
				squareGameData.EnPassantDirection = EnPassantDirection.BottomRight;
			}
		}
	}

	public override string ToString()
	{
		return coordinate.ToString();
	}
}
