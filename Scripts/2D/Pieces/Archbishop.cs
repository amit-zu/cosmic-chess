using static GameManager;

public partial class Archbishop : Piece
{
	public override void _Ready()
	{
		base._Ready();

		pieceProfile = GetPieceProfile<ArchbishopProfile>();
		AddSprite();

		GameManagerInstance.CurrentActiveGame.GetLastBoardState().SetBoardItemAt(currentSquare.GetFile(), currentSquare.GetRank(), (pieceProfile, color));
		GameManagerInstance.CurrentActiveGame.GetLastBoardState().DetectChecks();
	}
}
