using Godot;
using System;
using System.Collections.Generic;

public static class MiniChessPieceSprites
{
    private static readonly Dictionary<(Type, Color), string> pieceSpritePaths = new Dictionary<(Type, Color), string>()
    {
        { (typeof(KingProfile), Color.WHITE), "res://Assets/2D/Small Pieces/white_king_small.png" },
        { (typeof(QueenProfile), Color.WHITE), "res://Assets/2D/Small Pieces/white_queen_small.png" },
        { (typeof(RookProfile), Color.WHITE), "res://Assets/2D/Small Pieces/white_rook_small.png" },
        { (typeof(BishopProfile), Color.WHITE), "res://Assets/2D/Small Pieces/white_bishop_small.png" },
        { (typeof(KnightProfile), Color.WHITE), "res://Assets/2D/Small Pieces/white_knight_small.png" },
        { (typeof(PawnProfile), Color.WHITE), "res://Assets/2D/Small Pieces/white_pawn_small.png" },
        { (typeof(ArchbishopProfile), Color.WHITE), "res://Assets/2D/Small Pieces/white_archbishop_small.png" },

        { (typeof(KingProfile), Color.BLACK), "res://Assets/2D/Small Pieces/black_king_small.png" },
        { (typeof(QueenProfile), Color.BLACK), "res://Assets/2D/Small Pieces/black_queen_small.png" },
        { (typeof(RookProfile), Color.BLACK), "res://Assets/2D/Small Pieces/black_rook_small.png" },
        { (typeof(BishopProfile), Color.BLACK), "res://Assets/2D/Small Pieces/black_bishop_small.png" },
        { (typeof(KnightProfile), Color.BLACK), "res://Assets/2D/Small Pieces/black_knight_small.png" },
        { (typeof(PawnProfile), Color.BLACK), "res://Assets/2D/Small Pieces/black_pawn_small.png" },
        { (typeof(ArchbishopProfile), Color.BLACK), "res://Assets/2D/Small Pieces/black_archbishop_small.png" }
    };

    public static string GetSpritePath(PieceProfile pieceProfile, Color color)
    {
        Type profileType = pieceProfile.GetType();

        if (pieceSpritePaths.TryGetValue((profileType, color), out string path))
        {
            return path;
        }

        GD.PrintErr($"No sprite found for {color} {profileType.Name}");
        return "";
    }

    public static Texture2D LoadSprite(PieceProfile pieceProfile, Color color)
    {
        string path = GetSpritePath(pieceProfile, color);
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        return ResourceLoader.Load<Texture2D>(path);
    }
}