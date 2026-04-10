using static GameManager;

public partial class Knight : Piece
{
	public override void _Ready()
	{
		base._Ready();

		pieceProfile = GetPieceProfile<KnightProfile>();
		AddSprite();

		GameManagerInstance.CurrentActiveGame.GetLastBoardState().SetBoardItemAt(currentSquare.GetFile(), currentSquare.GetRank(), (pieceProfile, color));
		GameManagerInstance.CurrentActiveGame.GetLastBoardState().DetectChecks();
	}
}
