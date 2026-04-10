using System;
using Godot;

public partial class GameManager : Node
{
	public static readonly string DEFAULT_STARTING_POSITION_FEN_STRING = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
	public static bool IsDragging = false;
	public static GameManager GameManagerInstance { get; private set; }
	public Game CurrentActiveGame { get; private set; }
	public static SquareCoordinate[,] SquaresCoordinates = new SquareCoordinate[8, 8];
	public static EdgeCoordinate[,] EdgeCoordinates = new EdgeCoordinate[4, 8]; // Up Down Left Right
	public static readonly PieceProfile[] PieceProfiles = {
		new PawnProfile(),
		new KnightProfile(),
		new BishopProfile(),
		new ArchbishopProfile(),
		new RookProfile(),
		new QueenProfile(),
		new KingProfile(),
	};

	public static readonly BoardEdgeItemProfile[] BoardEdgeItemProfiles = {
		new PortalProfile(),
		new WallProfile()
	};

	public override void _Ready()
	{
		if (GameManagerInstance == null)
		{
			GameManagerInstance = this;
		}
		else
		{
			QueueFree();
		}

		InitializeSquareCoordinates();
		InitializeEdgeCoordinates();
	}

	public void SetGame(Game game)
	{
		CurrentActiveGame = game;
	}

	public static PieceProfile GetPieceProfile<T>() where T : PieceProfile
	{
		foreach (PieceProfile profile in PieceProfiles)
		{
			if (profile.GetType() == typeof(T))
			{
				return profile;
			}
		}
		GD.PrintErr($"No PieceLogic found for type {typeof(T)}");
		return null;
	}

	public static PieceProfile GetPieceProfileByType(Type pieceType)
	{

		foreach (PieceProfile profile in PieceProfiles)
		{
			if (profile.GetType() == pieceType)
			{
				return profile;
			}
		}

		return GetPieceProfile<PawnProfile>();
	}

	public static BoardEdgeItemProfile GetBoardEdgeItemProfile<T>() where T : BoardEdgeItemProfile
	{
		foreach (BoardEdgeItemProfile profile in BoardEdgeItemProfiles)
		{
			if (profile.GetType() == typeof(T))
			{
				return profile;
			}
		}
		GD.PrintErr($"No BoardEdgeItemProfile found for type {typeof(T)}");
		return null;
	}

	public static SquareCoordinate GetSquareCoordinate(File file, Rank rank)
	{
		if (file < File.A || file > File.H || rank < Rank.ONE || rank > Rank.EIGHT)
		{
			return null;
		}

		return SquaresCoordinates[(int)file - 1, (int)rank - 1];
	}

	public static EdgeCoordinate GetEdgeCoordinate(File file, Rank rank, Direction direction)
	{
		if (direction != Direction.Up && direction != Direction.Down && direction != Direction.Left && direction != Direction.Right)
		{
			return null;
		}

		foreach (EdgeCoordinate edgeCoordinate in EdgeCoordinates)
		{
			if (edgeCoordinate.File == file && edgeCoordinate.Rank == rank && edgeCoordinate.Direction == direction)
			{
				return edgeCoordinate;
			}
		}

		return null;
	}

	private void InitializeSquareCoordinates()
	{
		for (int file = 0; file < 8; file++)
		{
			for (int rank = 0; rank < 8; rank++)
			{
				SquaresCoordinates[file, rank] = new SquareCoordinate((File)(file + 1), (Rank)(rank + 1));
			}
		}
	}

	private void InitializeEdgeCoordinates()
	{
		for (File file = File.A; file <= File.H; file++)
		{
			EdgeCoordinates[0, (int)file - 1] = new EdgeCoordinate(file, Rank.EIGHT, Direction.Up); // a1 b1 c1 etc
		}

		for (File file = File.A; file <= File.H; file++)
		{
			EdgeCoordinates[1, (int)file - 1] = new EdgeCoordinate(file, Rank.ONE, Direction.Down); // a8 b8 c8 etc
		}

		for (Rank rank = Rank.ONE; rank <= Rank.EIGHT; rank++)
		{
			EdgeCoordinates[2, (int)rank - 1] = new EdgeCoordinate(File.A, rank, Direction.Left); // a1 a2 a3 etc
		}

		for (Rank rank = Rank.ONE; rank <= Rank.EIGHT; rank++)
		{
			EdgeCoordinates[3, (int)rank - 1] = new EdgeCoordinate(File.H, rank, Direction.Right); // h1 h2 h3 etc
		}
	}
}
