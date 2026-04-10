using static GameManager;

public partial class Rook : Piece
{
	public override void _Ready()
	{
		base._Ready();

		pieceProfile = GetPieceProfile<RookProfile>();
		AddSprite();

		GameManagerInstance.CurrentActiveGame.GetLastBoardState().SetBoardItemAt(currentSquare.GetFile(), currentSquare.GetRank(), (pieceProfile, color));
		GameManagerInstance.CurrentActiveGame.GetLastBoardState().DetectChecks();
	}
}
