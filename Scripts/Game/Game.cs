using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static GameManager;

public partial class Game : Node
{
	[Signal]
	public delegate void GameEndEventHandler(int gameConclusionType, int winnerColor);
	[Signal]
	public delegate void UpdateSideUIEventHandler();
	[Signal]
	public delegate void GetAIMoveEventHandler(string fenString);
	[Signal]
	public delegate void ChangeActionAvailabilityEventHandler();
	[Export]
	public Player PlayerWhite;
	[Export]
	public Player PlayerBlack;
	[Export]
	public string InitialFenString;
	[Export]
	public BotManager BotManager;
	public Color PlayerColor;
	public Color? WhoIsInCheck = null;
	public Color? CurrentTurn;
	public BoardState InitialBoardState;
	public List<BoardState> BoardStates;
	public List<Move> Moves;
	public int PlyCount = 0;
	public int MoveCount = 1;
	public EnPassantContext EnPassantContext;
	public CastlingContext CastlingContext;
	public Func<BoardState, bool> WinCondition;
	public GameType GameType;
	public ActionType PendingAction = ActionType.NONE;
	public EdgeCoordinate[] PendingPlacements = { null, null };
	public GameConclusionType ConclusionType = GameConclusionType.NONE;
	public File KingsideRookOriginalFile;
	public File QueensideRookOriginalFile;
	public static Func<Color, SquareCoordinate> GetKingDestinationForKingsideCastling = color =>
	new SquareCoordinate(color == Color.WHITE ? File.G : File.G, color == Color.WHITE ? Rank.ONE : Rank.EIGHT);

	public static Func<Color, SquareCoordinate> GetKingDestinationForQueensideCastling = color =>
		new SquareCoordinate(color == Color.WHITE ? File.C : File.C, color == Color.WHITE ? Rank.ONE : Rank.EIGHT);

	public static Func<Color, SquareCoordinate> GetRookDestinationForKingsideCastling = color =>
		new SquareCoordinate(color == Color.WHITE ? File.F : File.F, color == Color.WHITE ? Rank.ONE : Rank.EIGHT);

	public static Func<Color, SquareCoordinate> GetRookDestinationForQueensideCastling = color =>
		new SquareCoordinate(color == Color.WHITE ? File.D : File.D, color == Color.WHITE ? Rank.ONE : Rank.EIGHT);


	public override void _EnterTree()
	{
		if (string.IsNullOrEmpty(InitialFenString))
		{
			InitialFenString = DEFAULT_STARTING_POSITION_FEN_STRING;
		}

		InitialBoardState = new BoardState(InitialFenString);
		BoardStates = new List<BoardState>();
		Moves = new List<Move>();

		CurrentTurn = InitialFenString.Split(' ')[1] == "w" ? Color.WHITE : Color.BLACK;
		BoardStates.Add(InitialBoardState);

		GameManagerInstance.SetGame(this);

		SetCastlingContextFromFenString(InitialFenString);
		SetEnPassantContextFromFenString(InitialFenString);
		SetPlyCountFromFenString(InitialFenString);
		SetMoveCountFromFenString(InitialFenString);

		KingsideRookOriginalFile = GetOriginalKingsideRookFile();
		QueensideRookOriginalFile = GetOriginalQueensideRookFile();

		GameType = GameType.SelfPlay; // default value, will be overridden in _Ready of GameScreen
	}

	public override void _Ready()
	{

	}

	public string GetFenString()
	{
		return GetLastBoardState().GetFenStringFromBoardState() + " " + (CurrentTurn == Color.WHITE ? "w" : "b") + " " + CastlingContext.ToString() + " " + (EnPassantContext.Destination == null ? "-" : EnPassantContext.Destination.ToString()) + " " + PlyCount + " " + MoveCount;
	}

	public BoardState GetLastBoardState()
	{
		return BoardStates[BoardStates.Count - 1];
	}

	public Move GetLastMove()
	{
		if (Moves.Count == 0)
		{
			return null;
		}

		return Moves[Moves.Count - 1];
	}

	public BoardState UpdateBoardStateForMove(PieceProfile piece, Color color, SquareCoordinate initialSquare, SquareCoordinate move)
	{
		BoardState BoardStateAfterMove = GetLastBoardState().GetBoardStateAfterMove((piece, color),
		 initialSquare, move);
		BoardStates.Add(BoardStateAfterMove);
		return BoardStateAfterMove;
	}

	public BoardState UpdateBoardStateForPlacement(BoardEdgeItemProfile itemProfile, EdgeCoordinate edgeCoordinate, EdgeCoordinate? connectedEdgeItemCoordinate)
	{
		BoardState BoardStateAfterPlacement = GetLastBoardState().GetBoardStateAfterPlacement(itemProfile, edgeCoordinate, connectedEdgeItemCoordinate);
		if (connectedEdgeItemCoordinate != null)
		{
			BoardStateAfterPlacement.SetBoardEdgeItemAt(itemProfile, connectedEdgeItemCoordinate, edgeCoordinate);
		}

		BoardStates.Add(BoardStateAfterPlacement);
		return BoardStateAfterPlacement;
	}

	public BoardState UpdateBoardStateForPlacementOverride(BoardEdgeItemProfile itemProfile, EdgeCoordinate edgeCoordinate)
	{
		BoardState BoardStateAfterPlacementOverride = GetLastBoardState().GetBoardStateAfterPlacementOverride(itemProfile, edgeCoordinate);

		BoardStates.Add(BoardStateAfterPlacementOverride);
		return BoardStateAfterPlacementOverride;
	}

	public BoardState UpdateBoardStateForCastling(SquareCoordinate kingInitialSquare, SquareCoordinate kingDestinationSquare, SquareCoordinate rookInitialSquare, SquareCoordinate rookDestinationSquare, Color color)
	{
		BoardState BoardStateAfterMove = GetLastBoardState().GetBoardStateAfterMove((GetPieceProfile<KingProfile>(), color),
				 kingInitialSquare, kingDestinationSquare);
		BoardStateAfterMove.SetBoardItemAt(rookDestinationSquare.File, rookDestinationSquare.Rank, (GetPieceProfile<RookProfile>(), color));
		BoardStateAfterMove.SetBoardItemAt(rookInitialSquare.File, rookInitialSquare.Rank, null);
		BoardStates.Add(BoardStateAfterMove);

		return BoardStateAfterMove;
	}

	public BoardState UpdateBoardStateForPromotion(SquareCoordinate pawnSquareCoordinate, PieceProfile promotionPieceProfile, Color pawnColor)
	{
		GetLastBoardState().SetBoardItemAt(pawnSquareCoordinate.File, pawnSquareCoordinate.Rank, (promotionPieceProfile, pawnColor));
		return GetLastBoardState();
	}

	public BoardState UpdateBoardStateForEnPassant(SquareCoordinate capturedPawn, SquareCoordinate capturingPawnInitialSquare, SquareCoordinate destinationSquare, Color capturingPawnColor)
	{
		BoardState BoardStateAfterMove = GetLastBoardState().GetBoardStateAfterEnPassant(capturedPawn, capturingPawnInitialSquare, destinationSquare, capturingPawnColor);
		BoardStates.Add(BoardStateAfterMove);
		return BoardStateAfterMove;
	}

	public void SetGameType(GameType gameType)
	{
		GameType = gameType;
	}

	public void AddMove(Move move)
	{
		Moves.Add(move);
	}

	public void ChangeTurn()
	{
		CurrentTurn = Util.GetOppositeColor(CurrentTurn.Value);
		PlyCount++;

		if (CurrentTurn == Color.WHITE)
		{
			MoveCount++;
		}

		DetectCheck();
		DetectCheckmate();
		DetectStalemate();
		DetectLevelCleared(WinCondition);

		if (ConclusionType != GameConclusionType.NONE)
		{
			return;
		}

		ChangeClocks();

		EmitSignal(SignalName.UpdateSideUI);

		Move lastMove = GetLastMove();

		if (lastMove != null)
		{
			UpdateEnPassantContext(lastMove);
			UpdateCastlingContext(lastMove, GetLastBoardState());
		}

		//EmitSignal(SignalName.ChangeActionAvailability);

		if (CurrentTurn == Util.GetOppositeColor(PlayerColor))
		{
			if (GameType == GameType.Bot)
			{
				EmitSignal(SignalName.GetAIMove, GetFenString());
			}
		}

		GD.Print(GetFenString());
	}

	public void DetectCheckmate()
	{
		if (GetLastBoardState().WhoIsCheckmated() != null)
		{
			EmitSignal(SignalName.GameEnd, (int)GameConclusionType.CHECKMATE, (int)Util.GetOppositeColor(GetLastBoardState().WhoIsCheckmated()));
			ConclusionType = GameConclusionType.CHECKMATE;
			GameManagerInstance.CurrentActiveGame.EndGame();
		}
	}

	public void DetectCheck()
	{
		if (GameManagerInstance.CurrentActiveGame.GetLastBoardState().WhoIsInCheck.Count > 0)
		{
			GD.Print(GameManagerInstance.CurrentActiveGame.GetLastBoardState().WhoIsInCheck[0] + " Is in check");
		}
	}

	public void DetectStalemate()
	{
		if (GameManagerInstance.CurrentActiveGame.GetLastBoardState().IsStalemate(CurrentTurn.Value))
		{
			EmitSignal(SignalName.GameEnd, (int)GameConclusionType.STALEMATE, (int)Color.NONE);
			ConclusionType = GameConclusionType.STALEMATE;
			GameManagerInstance.CurrentActiveGame.EndGame();
		}
	}

	public void DetectLevelCleared(Func<BoardState, bool> winCondition)
	{
		if (winCondition == null) return;

		if (winCondition(GetLastBoardState()))
		{
			EmitSignal(SignalName.GameEnd, (int)GameConclusionType.LEVEL_CLEARED, (int)Color.NONE);
			ConclusionType = GameConclusionType.LEVEL_CLEARED;
		}
	}

	public void Timeout(Color lostPlayer)
	{
		EmitSignal(SignalName.GameEnd, (int)GameConclusionType.TIMEOUT, (int)lostPlayer);
		ConclusionType = GameConclusionType.TIMEOUT;
		GameManagerInstance.CurrentActiveGame.EndGame();
	}

	public void AddCapturedPieceToPlayer(Color colorOfCapturedPiece, PieceProfile pieceProfile)
	{
		if (colorOfCapturedPiece == Color.BLACK)
		{
			PlayerWhite.AddCapturedPiece(pieceProfile);
		}
		else if (colorOfCapturedPiece == Color.WHITE)
		{
			PlayerBlack.AddCapturedPiece(pieceProfile);
		}
	}

	public bool HasPieceMovedFromSquare(SquareCoordinate square)
	{
		var initialPiece = InitialBoardState.GetBoardItemAt(square);

		if (!initialPiece.HasValue)
		{
			return false;
		}

		return BoardStates.Any(boardState =>
		{
			var currentPiece = boardState.GetBoardItemAt(square);
			return !currentPiece.HasValue ||
				   currentPiece.Value.Item1 != initialPiece.Value.Item1 ||
				   currentPiece.Value.Item2 != initialPiece.Value.Item2;
		});
	}

	public List<SquareCoordinate> FindPawnsForEnPassant(SquareCoordinate enPassantDestinationSquare, Color movedPawnColor)
	{
		List<SquareCoordinate> enPassantPawns = new List<SquareCoordinate>();
		int rankIncrement = movedPawnColor == Color.WHITE ? 1 : -1;

		if (enPassantDestinationSquare.File > File.A)
		{
			(PieceProfile, Color)? boardItem = GetLastBoardState().GetBoardItemAt(GetSquareCoordinate(enPassantDestinationSquare.File - 1, enPassantDestinationSquare.Rank + rankIncrement));
			if (boardItem.HasValue && boardItem.Value.Item1 == GetPieceProfile<PawnProfile>() && boardItem.Value.Item2 == Util.GetOppositeColor(movedPawnColor))
			{
				enPassantPawns.Add(GetSquareCoordinate(enPassantDestinationSquare.File - 1, enPassantDestinationSquare.Rank + rankIncrement));
			}
		}
		if (enPassantDestinationSquare.File < File.H)
		{
			(PieceProfile, Color)? boardItem = GetLastBoardState().GetBoardItemAt(GetSquareCoordinate(enPassantDestinationSquare.File + 1, enPassantDestinationSquare.Rank + rankIncrement));
			if (boardItem.HasValue && boardItem.Value.Item1 == GetPieceProfile<PawnProfile>() && boardItem.Value.Item2 == Util.GetOppositeColor(movedPawnColor))
			{
				enPassantPawns.Add(GetSquareCoordinate(enPassantDestinationSquare.File + 1, enPassantDestinationSquare.Rank + rankIncrement));
			}
		}

		return enPassantPawns;
	}

	public void UpdateEnPassantContext(Move lastMove)
	{
		EnPassantContext = new EnPassantContext();

		if (lastMove.PieceProfile is PawnProfile && Math.Abs((int)lastMove.DestinationSquare.Rank - (int)lastMove.OriginSquare.Rank) == 2)
		{
			SquareCoordinate enPassantDestination = lastMove.Color == Color.WHITE
				? new SquareCoordinate(lastMove.DestinationSquare.File, lastMove.DestinationSquare.Rank - 1)
				: new SquareCoordinate(lastMove.DestinationSquare.File, lastMove.DestinationSquare.Rank + 1);

			EnPassantContext.Destination = enPassantDestination;
			EnPassantContext.Ply = PlyCount;

			List<SquareCoordinate> enPassantPawns = FindPawnsForEnPassant(EnPassantContext.Destination, lastMove.Color);

			if (enPassantPawns.Count > 0)
			{
				EnPassantContext.EligibleSquares = enPassantPawns;
				EnPassantContext.CapturedPawnCoordinate = lastMove.DestinationSquare;
			}
		}
	}

	public void UpdateCastlingContext(Move move, BoardState boardStateAfterMove)
	{
		// section 1: if a king or rook moves, update castling rights

		if (move.PieceProfile is KingProfile)
		{
			if (move.Color == Color.WHITE)
			{
				CastlingContext.WhiteCanCastleKingside = false;
				CastlingContext.WhiteCanCastleQueenside = false;
			}
			else if (move.Color == Color.BLACK)
			{
				CastlingContext.BlackCanCastleKingside = false;
				CastlingContext.BlackCanCastleQueenside = false;
			}
		}
		else if (move.PieceProfile is RookProfile)
		{
			if (move.Color == Color.WHITE)
			{
				if (move.OriginSquare == GetSquareCoordinate(KingsideRookOriginalFile, Rank.ONE))
				{
					CastlingContext.WhiteCanCastleKingside = false;
				}
				else if (move.OriginSquare == GetSquareCoordinate(QueensideRookOriginalFile, Rank.ONE))
				{
					CastlingContext.WhiteCanCastleQueenside = false;
				}
			}
			else if (move.Color == Color.BLACK)
			{
				if (move.OriginSquare == GetSquareCoordinate(KingsideRookOriginalFile, Rank.ONE))
				{
					CastlingContext.BlackCanCastleKingside = false;
				}
				else if (move.OriginSquare == GetSquareCoordinate(QueensideRookOriginalFile, Rank.ONE))
				{
					CastlingContext.BlackCanCastleQueenside = false;
				}
			}
		}

		// section 2: if a rook is captured, update castling rights

		SquareCoordinate kingsideRookSquare = boardStateAfterMove.FindKingSideRook(move.Color);
		SquareCoordinate queensideRookSquare = boardStateAfterMove.FindQueenSideRook(move.Color);

		if (move.Color == Color.WHITE)
		{
			if (kingsideRookSquare == null)
			{
				CastlingContext.WhiteCanCastleKingside = false;
			}
			if (queensideRookSquare == null)
			{
				CastlingContext.WhiteCanCastleQueenside = false;
			}
		}
		else if (move.Color == Color.BLACK)
		{
			if (kingsideRookSquare == null)
			{
				CastlingContext.BlackCanCastleKingside = false;
			}
			if (queensideRookSquare == null)
			{
				CastlingContext.BlackCanCastleQueenside = false;
			}
		}

	}

	public File GetOriginalKingsideRookFile()
	{
		BoardState firstBoardState = BoardStates[0];
		for (File file = File.H; file >= File.A; file--)
		{
			(PieceProfile, Color)? boardItem = firstBoardState.GetBoardItemAt(GetSquareCoordinate(file, Rank.ONE));
			if (boardItem.HasValue && boardItem.Value.Item1 is RookProfile && boardItem.Value.Item2 == Color.WHITE)
			{
				return file;
			}


		}

		return File.A; // should never happen
	}

	public File GetOriginalQueensideRookFile()
	{
		BoardState firstBoardState = BoardStates[0];

		for (File file = File.A; file <= File.H; file++)
		{
			(PieceProfile, Color)? boardItem = firstBoardState.GetBoardItemAt(GetSquareCoordinate(file, Rank.ONE));
			if (boardItem.HasValue && boardItem.Value.Item1 is RookProfile && boardItem.Value.Item2 == Color.WHITE)
			{
				return file;
			}
		}

		return File.A; // should never happen
	}

	public void EndGame()
	{
		CurrentTurn = Color.NONE;
	}

	public override string ToString()
	{
		return "-"; //todo complete this
	}

	private void ChangeClocks()
	{
		if (CurrentTurn == Color.WHITE)
		{
			PlayerWhite.StartClock();
			PlayerBlack.StopClock();
		}
		else if (CurrentTurn == Color.BLACK)
		{
			PlayerBlack.StartClock();
			PlayerWhite.StopClock();
		}
		else
		{
			PlayerWhite.StopClock();
			PlayerBlack.StopClock();
		}
	}

	private void SetCastlingContextFromFenString(string fenString)
	{
		CastlingContext = new CastlingContext();
		string castlingRights = fenString.Split(' ')[2];
		CastlingContext.WhiteCanCastleKingside = castlingRights.Contains('K');
		CastlingContext.WhiteCanCastleQueenside = castlingRights.Contains('Q');
		CastlingContext.BlackCanCastleKingside = castlingRights.Contains('k');
		CastlingContext.BlackCanCastleQueenside = castlingRights.Contains('q');
	}

	private void SetEnPassantContextFromFenString(string fenString)
	{
		EnPassantContext = new EnPassantContext();
		string enPassantSquareString = fenString.Split(' ')[3];
		if (enPassantSquareString != "-")
		{
			EnPassantContext.Destination = SquareCoordinate.FromString(enPassantSquareString);
			EnPassantContext.Ply = 0;
			EnPassantContext.EligibleSquares = FindPawnsForEnPassant(EnPassantContext.Destination, Util.GetOppositeColor(CurrentTurn.Value));
			if (EnPassantContext.EligibleSquares.Count > 0)
			{
				//EnPassantContext.CapturedPawnCoordinate = new SquareCoordinate(EnPassantContext.Destination.File, CurrentTurn == Color.WHITE ? EnPassantContext.Destination.Rank - 1 : EnPassantContext.Destination.Rank + 1);
				EnPassantContext.CapturedPawnCoordinate = GameManager.GetSquareCoordinate(EnPassantContext.Destination.File, CurrentTurn == Color.WHITE ? EnPassantContext.Destination.Rank - 1 : EnPassantContext.Destination.Rank + 1);
			}
		}
	}

	private void SetPlyCountFromFenString(string fenString)
	{
		string plyCountString = fenString.Split(' ')[4];
		if (int.TryParse(plyCountString, out int plyCount))
		{
			PlyCount = plyCount;
		}
		else
		{
			PlyCount = 0;
		}
	}

	private void SetMoveCountFromFenString(string fenString)
	{
		string moveCountString = fenString.Split(' ')[5];
		if (int.TryParse(moveCountString, out int moveCount))
		{
			MoveCount = moveCount;
		}
		else
		{
			MoveCount = 1;
		}
	}
}
