using System.Collections.Generic;
using static GameManager;

public partial class King : Piece
{
	protected List<SquareCoordinate> CastlingSquares;
	protected CastlingContext castlingContext;

	public override void _Ready()
	{
		base._Ready();

		pieceProfile = GetPieceProfile<KingProfile>();
		castlingContext = GameManagerInstance.CurrentActiveGame.CastlingContext;
		AddSprite();

		CastlingSquares = new List<SquareCoordinate>();

		GameManagerInstance.CurrentActiveGame.GetLastBoardState().SetBoardItemAt(currentSquare.GetFile(), currentSquare.GetRank(), (pieceProfile, color));
		GameManagerInstance.CurrentActiveGame.GetLastBoardState().DetectChecks();
	}

	protected override void OnClick()
	{
		base.OnClick();

		if (GameManagerInstance.CurrentActiveGame.CurrentTurn == color)
		{
			CastlingSquares = ((KingProfile)pieceProfile).GetCastlingSquares(GameManagerInstance.CurrentActiveGame.GetLastBoardState(), currentSquare.GetCoordinate(), castlingContext);
		}
	}

	protected override void OnDrag()
	{
		base.OnDrag();

		board.MarkCastlingSquares(CastlingSquares.ToArray());
	}

	protected override void OnRelease()
	{
		base.OnRelease();

		board.UnmarkAllSquares();

		IsDragging = false;

		if (Util.ContainsSquare(CastlingSquares, hoveredSquare.GetCoordinate()) && GameManagerInstance.CurrentActiveGame.CurrentTurn == color && GameManagerInstance.CurrentActiveGame.PendingAction == ActionType.NONE)
		{
			previousSquare = currentSquare;

			CastlingType castlingType = Game.GetKingDestinationForKingsideCastling(color).Equals(hoveredSquare.GetCoordinate())
			? CastlingType.KINGSIDE : CastlingType.QUEENSIDE;

			EmitSignal(SignalName.Castle, previousSquare.GetCoordinate(), hoveredSquare.GetCoordinate(), (int)castlingType, (int)color);

			hasMoved = true;
		}

		CastlingSquares.Clear();
	}
}
