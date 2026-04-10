using System;
using System.Collections.Generic;
using Godot;

public static class BoardItemInfo
{
	private static readonly Dictionary<PieceProfile, char> _symbols = new Dictionary<PieceProfile, char>
	{
		{ GameManager.GetPieceProfile<KingProfile>(), 'K' },
		{ GameManager.GetPieceProfile<RookProfile>(), 'R' },
		{ GameManager.GetPieceProfile<PawnProfile>(), 'P' },
		{ GameManager.GetPieceProfile<KnightProfile>(), 'N'},
		{ GameManager.GetPieceProfile<BishopProfile>(), 'B' },
		{ GameManager.GetPieceProfile<ArchbishopProfile>(), 'A' },
		{ GameManager.GetPieceProfile<QueenProfile>(), 'Q' }
	};

	private static readonly Dictionary<char, PieceProfile> _typesFromSymbol = new Dictionary<char, PieceProfile>
{
	{ 'K', GameManager.GetPieceProfile<KingProfile>() },
	{ 'R', GameManager.GetPieceProfile<RookProfile>() },
	{ 'P', GameManager.GetPieceProfile<PawnProfile>() },
	{ 'N', GameManager.GetPieceProfile<KnightProfile>() },
	{ 'B', GameManager.GetPieceProfile<BishopProfile>() },
	{ 'A', GameManager.GetPieceProfile<ArchbishopProfile>() },
	{ 'Q', GameManager.GetPieceProfile<QueenProfile>() }
};

	private static readonly Dictionary<PieceProfile, Texture2D> _whiteSprites = new Dictionary<PieceProfile, Texture2D>
	{
		{ GameManager.GetPieceProfile<KingProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/white_king.png") },
		{ GameManager.GetPieceProfile<RookProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/white_rook.png") },
		{ GameManager.GetPieceProfile<PawnProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/white_pawn.png") },
		{ GameManager.GetPieceProfile<KnightProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/white_knight.png")},
		{ GameManager.GetPieceProfile<BishopProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/white_bishop.png") },
		{ GameManager.GetPieceProfile<ArchbishopProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/white_archbishop.png") },
		{ GameManager.GetPieceProfile<QueenProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/white_queen.png") }
	};

	private static readonly Dictionary<PieceProfile, Texture2D> _blackSprites = new Dictionary<PieceProfile, Texture2D>
	{
		{ GameManager.GetPieceProfile<KingProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/black_king.png") },
		{ GameManager.GetPieceProfile<RookProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/black_rook.png") },
		{ GameManager.GetPieceProfile<PawnProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/black_pawn.png") },
		{ GameManager.GetPieceProfile<KnightProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/black_knight.png")},
		{ GameManager.GetPieceProfile<BishopProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/black_bishop.png") },
		{ GameManager.GetPieceProfile<ArchbishopProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/black_archbishop.png") },
		{ GameManager.GetPieceProfile<QueenProfile>(), ResourceLoader.Load<Texture2D>("res://Assets/2D/Pieces/black_queen.png") }
	};

	private static readonly Dictionary<PieceProfile, String> _2dscanePaths
	 = new Dictionary<PieceProfile, String>
	{
		{ GameManager.GetPieceProfile<KingProfile>(), "res://Scenes/2D/Pieces/king_2d.tscn" },
		{ GameManager.GetPieceProfile<RookProfile>(), "res://Scenes/2D/Pieces/rook_2d.tscn" },
		{ GameManager.GetPieceProfile<PawnProfile>(), "res://Scenes/2D/Pieces/pawn_2d.tscn" },
		{ GameManager.GetPieceProfile<KnightProfile>(), "res://Scenes/2D/Pieces/knight_2d.tscn"},
		{ GameManager.GetPieceProfile<BishopProfile>(), "res://Scenes/2D/Pieces/bishop_2d.tscn" },
		{ GameManager.GetPieceProfile<ArchbishopProfile>(), "res://Scenes/2D/Pieces/archbishop_2d.tscn" },
		{ GameManager.GetPieceProfile<QueenProfile>(), "res://Scenes/2D/Pieces/queen_2d.tscn" }
	};

	public static char GetSymbol(PieceProfile profile)
	{
		return _symbols.TryGetValue(profile, out char symbol) ? symbol : '?';
	}

	public static PieceProfile GetProfileFromSymbol(Char c)
	{
		if (!Char.IsLetter(c))
		{
			return null;
		}

		return _typesFromSymbol.TryGetValue(c, out PieceProfile profile) ? profile : null;
	}

	public static Texture2D GetSprite(PieceProfile pieceProfile, Color color)
	{
		return color == Color.WHITE ? (_whiteSprites.TryGetValue(pieceProfile, out Texture2D whiteTexture) ? whiteTexture : null)
		: (_blackSprites.TryGetValue(pieceProfile, out Texture2D blackTexture) ? blackTexture : null);
	}

	public static string Get2DScenePath(PieceProfile pieceProfile)
	{
		return _2dscanePaths.TryGetValue(pieceProfile, out string path) ? path : null;
	}
}
