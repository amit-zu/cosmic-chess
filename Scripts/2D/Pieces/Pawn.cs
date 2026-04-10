using Godot;
using static GameManager;

public partial class Pawn : Piece
{
	protected SquareCoordinate enPassantSquare;
	protected SquareCoordinate enPassantCapturedPawnCoordinate;

	public override void _Ready()
	{
		base._Ready();

		pieceProfile = GetPieceProfile<PawnProfile>();
		AddSprite();

		GameManagerInstance.CurrentActiveGame.GetLastBoardState().SetBoardItemAt(currentSquare.GetFile(), currentSquare.GetRank(), (pieceProfile, color));
		GameManagerInstance.CurrentActiveGame.GetLastBoardState().DetectChecks();
	}

	protected override void OnClick()
	{
		base.OnClick();

		if (GameManagerInstance.CurrentActiveGame.EnPassantContext.Destination != null && GameManagerInstance.CurrentActiveGame.EnPassantContext.Ply == GameManagerInstance.CurrentActiveGame.PlyCount
				&& Util.ContainsSquare(GameManagerInstance.CurrentActiveGame.EnPassantContext.EligibleSquares, currentSquare.GetCoordinate()))
		{
			enPassantCapturedPawnCoordinate = GameManagerInstance.CurrentActiveGame.EnPassantContext.CapturedPawnCoordinate;

			if (!GameManagerInstance.CurrentActiveGame.GetLastBoardState().GetBoardStateAfterEnPassant(enPassantCapturedPawnCoordinate,
			currentSquare.GetCoordinate(), GameManagerInstance.CurrentActiveGame.EnPassantContext.Destination, color).WhoIsInCheck.Contains(color))
			{
				enPassantSquare = GameManagerInstance.CurrentActiveGame.EnPassantContext.Destination;
				board.MarkEnPassantSquare(enPassantSquare, currentSquare.GetCoordinate(), color);
			}
		}
	}

	protected override void OnRelease()
	{
		base.OnRelease();

		board.UnmarkAllSquares();

		IsDragging = false;

		if (enPassantSquare != null && enPassantSquare.Equals(hoveredSquare.GetCoordinate()) && GameManagerInstance.CurrentActiveGame.CurrentTurn == color && GameManagerInstance.CurrentActiveGame.PendingAction == ActionType.NONE)
		{
			previousSquare = currentSquare;

			EmitSignal(SignalName.MovePiece, previousSquare.GetCoordinate(), hoveredSquare.GetCoordinate(), pieceProfile, (int)color);
			EmitSignal(SignalName.CapturePiece, enPassantCapturedPawnCoordinate);

			GameManagerInstance.CurrentActiveGame.UpdateBoardStateForEnPassant(
				enPassantCapturedPawnCoordinate,
				previousSquare.GetCoordinate(),
				enPassantSquare,
				color
			);

			enPassantSquare = null;
		}
	}

}
