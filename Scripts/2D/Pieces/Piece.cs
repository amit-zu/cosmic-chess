using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static GameManager;

[GlobalClass]
public partial class Piece : BoardItem
{
	// signals
	[Signal]
	public delegate void PromotionEventHandler(SquareCoordinate pawnOriginSquare, SquareCoordinate promotionSquare, int color);
	[Signal]
	public delegate void MovePieceEventHandler(SquareCoordinate initialSquare, SquareCoordinate finalSquare, PieceProfile pieceProfile, int color);
	[Signal]
	public delegate void CapturePieceEventHandler(SquareCoordinate pieceCoordinate);
	[Signal]
	public delegate void CastleEventHandler(SquareCoordinate initialSquare, SquareCoordinate finalSquare, int castlingType, int color);

	// *************************************

	protected Color color;
	protected File startingFile;
	protected Rank startingRank;
	protected Board board;
	protected PieceProfile pieceProfile;
	protected List<SquareCoordinate> LegalSquares;
	protected List<SquareCoordinate> TakingSquares;
	protected bool hasMoved = false;
	protected Square startingSquare;
	protected Square currentSquare;
	protected Square previousSquare;

	public override void _Ready()
	{
		base._Ready();

		Position = currentSquare.Position;
		LegalSquares = new List<SquareCoordinate>();
		TakingSquares = new List<SquareCoordinate>();
	}

	public void SetBoard(Board board)
	{
		this.board = board;
	}

	public Square GetSquare()
	{
		return currentSquare;
	}

	public Square GetPreviousSquare()
	{
		return previousSquare;
	}

	public void SetSquare(Square square)
	{
		currentSquare = square;
		currentSquare.SetOccupyingPiece(this);
	}

	public void SetSquare(Square previousSquare, Square nextSquare)
	{
		previousSquare.ClearOccupyingPiece();

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "position", nextSquare.Position, 0.2).SetEase(Tween.EaseType.Out);
		currentSquare = nextSquare;
		currentSquare.SetOccupyingPiece(this);
	}

	public void SetInitialSquare(Square square)
	{
		startingSquare = square;
		currentSquare = square;
	}

	public File GetStartingFile()
	{
		return startingFile;
	}

	public Rank GetStartingRank()
	{
		return startingRank;
	}

	public void SetStartingFile(File file)
	{
		startingFile = file;
	}

	public void SetStartingRank(Rank rank)
	{
		startingRank = rank;
	}

	public Color GetColor()
	{
		return color;
	}

	public void SetColor(Color color)
	{
		this.color = color;
	}

	public PieceProfile GetProfile()
	{
		return pieceProfile;
	}

	public void ClearLegalSquares()
	{
		LegalSquares.Clear();
	}

	public void ClearTakingSquares()
	{
		TakingSquares.Clear();
	}

	public void Enable()
	{
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	public void Disable()
	{
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
	}
	public override string ToString()
	{
		return currentSquare.ToString() + color.ToString() + GetType().Name;
	}

	public static List<SquareCoordinate> FilterLegalSquaresForCheck(BoardState boardState, SquareCoordinate initialSquare, (PieceProfile, Color) boardItem, List<SquareCoordinate> squares)
	{
		return squares.Where(square => !boardState.GetBoardStateAfterMove(boardItem, initialSquare, square).WhoIsInCheck.Contains(boardItem.Item2)).ToList();
	}

	protected override void OnClick()
	{
		offset = GetGlobalMousePosition() - GlobalPosition;
		IsDragging = true;

		if (GameManagerInstance.CurrentActiveGame.CurrentTurn == color && GameManagerInstance.CurrentActiveGame.PendingAction == ActionType.NONE)
		{
			LegalSquares = pieceProfile.GetLegalSquares(GameManagerInstance.CurrentActiveGame.GetLastBoardState(), currentSquare.GetCoordinate(), hasMoved);
			TakingSquares = pieceProfile.GetTakingSquares(GameManagerInstance.CurrentActiveGame.GetLastBoardState(), currentSquare.GetCoordinate());
		}
	}

	protected override void OnDrag()
	{
		GlobalPosition = GetGlobalMousePosition() - offset;
		board.MarkLegalSquares(LegalSquares.ToArray());
		board.MarkTakingSquares(TakingSquares.ToArray());
	}

	protected override void OnRelease()
	{
		base.OnRelease();

		board.UnmarkAllSquares();

		IsDragging = false;

		if (Util.ContainsSquare(LegalSquares, hoveredSquare?.GetCoordinate()) && GameManagerInstance.CurrentActiveGame.CurrentTurn == color && GameManagerInstance.CurrentActiveGame.PendingAction == ActionType.NONE)
		{
			previousSquare = currentSquare;

			if (pieceProfile == GetPieceProfile<PawnProfile>())
			{
				Rank promotionRank = (color == Color.WHITE) ? Rank.EIGHT : Rank.ONE;

				if (hoveredSquare.GetCoordinate().Rank == promotionRank)
				{
					EmitSignal(SignalName.Promotion, previousSquare.GetCoordinate(), hoveredSquare.GetCoordinate(), (int)color);
				}
				else
				{
					EmitSignal(SignalName.MovePiece, previousSquare.GetCoordinate(), hoveredSquare.GetCoordinate(), pieceProfile, (int)color);
				}
			}
			else
			{
				EmitSignal(SignalName.MovePiece, previousSquare.GetCoordinate(), hoveredSquare.GetCoordinate(), pieceProfile, (int)color);
			}

			hasMoved = true;
		}
		else if (Util.ContainsSquare(TakingSquares, hoveredSquare?.GetCoordinate()) && GameManagerInstance.CurrentActiveGame.CurrentTurn == color && GameManagerInstance.CurrentActiveGame.PendingAction == ActionType.NONE)
		{
			previousSquare = currentSquare;

			GameManagerInstance.CurrentActiveGame.AddCapturedPieceToPlayer(Util.GetOppositeColor(color), hoveredSquare.GetOccupyingPiece().GetProfile());
			EmitSignal(SignalName.CapturePiece, hoveredSquare.GetCoordinate());


			if (pieceProfile == GetPieceProfile<PawnProfile>())
			{
				Rank promotionRank = (color == Color.WHITE) ? Rank.EIGHT : Rank.ONE;

				if (hoveredSquare.GetCoordinate().Rank == promotionRank)
				{
					GameManagerInstance.CurrentActiveGame.PendingAction = ActionType.PROMOTION;
					EmitSignal(SignalName.Promotion, hoveredSquare.GetCoordinate());
				}
				else
				{
					EmitSignal(SignalName.MovePiece, previousSquare.GetCoordinate(), hoveredSquare.GetCoordinate(), pieceProfile, (int)color);
				}
			}
			else
			{
				EmitSignal(SignalName.MovePiece, previousSquare.GetCoordinate(), hoveredSquare.GetCoordinate(), pieceProfile, (int)color);
			}

			hasMoved = true;
		}
		else
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(this, "position", currentSquare.GlobalPosition, 0.2).SetEase(Tween.EaseType.Out);
		}

		ClearLegalSquares();
		ClearTakingSquares();

	}

	protected void AddSprite()
	{
		Sprite2D sprite = new Sprite2D
		{
			Scale = new Vector2(0.4F, 0.5F),
			Position = new Vector2(0F, -2F)
		};

		sprite.Texture = BoardItemInfo.GetSprite(pieceProfile, color);

		AddChild(sprite);
	}
}
