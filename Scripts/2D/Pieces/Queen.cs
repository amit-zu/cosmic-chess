using static GameManager;

public partial class Queen : Piece
{
	public override void _Ready()
	{
		base._Ready();

		pieceProfile = GetPieceProfile<QueenProfile>();
		AddSprite();

		GameManagerInstance.CurrentActiveGame.GetLastBoardState().SetBoardItemAt(currentSquare.GetFile(), currentSquare.GetRank(), (pieceProfile, color));
		GameManagerInstance.CurrentActiveGame.GetLastBoardState().DetectChecks();
	}
}
