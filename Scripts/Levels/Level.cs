using System.Collections.Generic;
using Godot;

public partial class Level : GameScreen
{

	[Export]
	public int EloLevel;
	[Export]
	public string GoalDescription;
	protected Queue<DialogueLine> dialogueSequence;
	protected Timer moveTimer;
	private string pendingBotMove;
	private BoardState pendingBoardState;
	private bool hoveringOnDialoguePanel = false;
	private string currentDialogueText = "";
	private int currentCharIndex = 0;
	private DialogueLine currentDialogueLine;
	private bool isAnimatingText = false;

	public override void _Ready()
	{
		base._Ready();
		BotManager.SetEloLevel(EloLevel);

		dialogueSequence = new Queue<DialogueLine>();
		moveTimer = GetNode<Timer>("Game/MoveTimer");
		moveTimer.WaitTime = 0.5f;
		moveTimer.Timeout += ExecutePendingMove;

		Game.SetGameType(GameType.Bot);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Input.IsActionJustPressed("click") && hoveringOnDialoguePanel)
		{
			ShowNextDialogueLine();
		}
	}

	public void StartDialogue()
	{
		if (dialogueSequence == null || dialogueSequence.Count == 0)
		{
			GD.Print("No dialogue to show.");
			return;
		}

		ShowNextDialogueLine();
	}

	public void ShowNextDialogueLine() // TODO change sprite for each speaker
	{
		Panel characterPanel = GetNode<Panel>("UICanvasLayer/UI/CharacterPanel");
		RichTextLabel dialogueLabel = GetNode<RichTextLabel>("UICanvasLayer/UI/CharacterPanel/CharacterRect/DialoguePanel/DialogueLabel");
		TextureRect portraitRect = GetNode<TextureRect>("UICanvasLayer/UI/CharacterPanel/CharacterRect");
		RichTextLabel speakerNameLabel = GetNode<RichTextLabel>("UICanvasLayer/UI/CharacterPanel/CharacterRect/NamePanel/NameLabel");

		if (isAnimatingText)
		{
			if (currentDialogueLine != null && currentDialogueLine.Skippable)
			{
				isAnimatingText = false;
				dialogueLabel.Text = currentDialogueText;
			}

			return;
		}

		if (dialogueSequence.Count == 0)
		{
			EnableGameInteraction();
			ResumeClocks();
			ShowGoalDescription();
			characterPanel.Visible = false;
			return;
		}

		currentDialogueLine = dialogueSequence.Dequeue();
		currentDialogueText = currentDialogueLine.Text;
		speakerNameLabel.Text = currentDialogueLine.SpeakerName;

		if (currentDialogueLine.Shader != null)
		{
			portraitRect.Material = new ShaderMaterial { Shader = currentDialogueLine.Shader };
		}
		else
		{
			portraitRect.Material = null;
		}

		currentCharIndex = 0;
		dialogueLabel.Text = "";
		isAnimatingText = true;

		Timer textAnimTimer = new Timer();
		textAnimTimer.WaitTime = 0.05;
		textAnimTimer.OneShot = false;

		textAnimTimer.Timeout += () =>
		{
			if (!isAnimatingText || currentCharIndex >= currentDialogueText.Length)
			{
				textAnimTimer.Stop();
				textAnimTimer.QueueFree();
				isAnimatingText = false;
				return;
			}

			currentCharIndex++;
			dialogueLabel.Text = currentDialogueText.Substring(0, currentCharIndex);

			// Play typing sound (if you have one)
			// AudioStreamPlayer typingSound = GetNode<AudioStreamPlayer>("TypingSound");
			// typingSound.Play();
		};

		AddChild(textAnimTimer);
		textAnimTimer.Start();
	}

	public void ShowGoalDescription()
	{
		RichTextLabel objectiveLabel = GetNode<RichTextLabel>("UICanvasLayer/UI/RightPanel/GoalPanel/ObjectiveLabel");

		objectiveLabel.Text = GoalDescription;
	}

	public void OnMouseEnteredDialoguePanel()
	{
		hoveringOnDialoguePanel = true;
	}

	public void OnMouseExitedDialoguePanel()
	{
		hoveringOnDialoguePanel = false;
	}

	public async void PlayBotMove(string fenString)
	{
		string botMoveUCI = await BotManager.GetMove(fenString);
		pendingBotMove = botMoveUCI;
		pendingBoardState = Game.GetLastBoardState();

		moveTimer.Start();
	}

	private void ExecutePendingMove()
	{
		moveTimer.Stop();

		if (string.IsNullOrEmpty(pendingBotMove))
		{
			return;
		}

		if (Game.GetLastBoardState().Equals(pendingBoardState))
		{
			Move move = GetMoveFromUCI(pendingBotMove);
			MoveManager.MakeMove(move);
		}
		else
		{
			GD.Print("Board state changed - canceling bot move");
		}

		pendingBotMove = null;
		pendingBoardState = null;
	}

	private Move GetMoveFromUCI(string uciString)
	{
		BoardState boardState = Game.GetLastBoardState();
		EnPassantContext enPassantContext = Game.EnPassantContext;
		CastlingContext castlingContext = Game.CastlingContext;

		SquareCoordinate initialSquare = Util.GetCoordinateFromString(uciString.Substring(0, 2));
		SquareCoordinate finalSquare = Util.GetCoordinateFromString(uciString.Substring(2, 2));

		(PieceProfile, Color)? movedPiece = boardState.GetBoardItemAt(initialSquare);
		PieceProfile capturedPieceProfile = boardState.GetBoardItemAt(finalSquare)?.Item1 ?? null;
		PieceProfile promotionPiece = null;

		if (movedPiece.Value.Item1 == GameManager.GetPieceProfile<KingProfile>())
		{
			Color playerColor = movedPiece.Value.Item2;

			if (finalSquare.Equals(Game.GetKingDestinationForKingsideCastling(playerColor)))
			{
				bool canCastleKingside = (playerColor == Color.WHITE) ?
					castlingContext.WhiteCanCastleKingside :
					castlingContext.BlackCanCastleKingside;

				if (canCastleKingside)
				{
					return new Move(movedPiece.Value.Item1, initialSquare, finalSquare,
								   playerColor, null, null, CastlingType.KINGSIDE);
				}
			}

			else if (finalSquare.Equals(Game.GetKingDestinationForQueensideCastling(playerColor)))
			{
				bool canCastleQueenside = (playerColor == Color.WHITE) ?
					castlingContext.WhiteCanCastleQueenside :
					castlingContext.BlackCanCastleQueenside;

				if (canCastleQueenside)
				{
					return new Move(movedPiece.Value.Item1, initialSquare, finalSquare,
								   playerColor, null, null, CastlingType.QUEENSIDE);
				}
			}
		}

		if (uciString.Length == 5)
		{
			promotionPiece = BoardItemInfo.GetProfileFromSymbol(uciString[4].ToString().ToUpper()[0]);
		}

		return new Move(movedPiece.Value.Item1, initialSquare, finalSquare,
					   movedPiece.Value.Item2, capturedPieceProfile, promotionPiece);
	}
}
