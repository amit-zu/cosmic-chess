using Godot;

public partial class Level1 : Level
{
	public override void _Ready()
	{
		base._Ready();

		Game.WinCondition = (BoardState boardState) =>
		{
			for (File file = File.A; file <= File.H; file++)
			{
				(PieceProfile, Color)? pieceAtSquare = boardState.GetBoardItemAt(new SquareCoordinate(file, Rank.EIGHT));

				if (pieceAtSquare == null) continue;


				if (pieceAtSquare.Value.Item2 == Color.WHITE && pieceAtSquare.Value.Item1 != GameManager.GetPieceProfile<PawnProfile>()
					&& pieceAtSquare.Value.Item1 != GameManager.GetPieceProfile<KingProfile>())
				{
					return true;
				}
			}

			return false;
		};

		dialogueSequence.Enqueue(new DialogueLine()
		{
			SpeakerName = "Mysterious Figure",
			Text = "Hello? Is anyone there?",
			Portrait = GD.Load<Texture2D>("res://Assets/2D/Characters/good_king.png"),
			Skippable = false,
			Shader = GD.Load<Shader>("res://Scripts/Shaders/darken.gdshader"),
		});

		dialogueSequence.Enqueue(new DialogueLine()
		{
			SpeakerName = "King Arthur",
			Text = "Some meaningful text",
			Portrait = GD.Load<Texture2D>("res://Assets/2D/Characters/good_king.png"),
			Skippable = false,
		});

		DisableGameInteraction();
		PauseClocks();

		StartDialogue();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
