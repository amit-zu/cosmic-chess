using System;
using System.Collections.Generic;
using Godot;

public static class BoardEdgeItemInfo
{
    private static readonly Dictionary<BoardEdgeItemProfile, char> _symbols = new Dictionary<BoardEdgeItemProfile, char>
    {
        { GameManager.GetBoardEdgeItemProfile<PortalProfile>(), 'P' },
        { GameManager.GetBoardEdgeItemProfile<WallProfile>(), 'W' }
    };

    private static readonly Dictionary<char, BoardEdgeItemProfile> _typesFromSymbol = new Dictionary<char, BoardEdgeItemProfile>
    {
        { 'P', GameManager.GetBoardEdgeItemProfile<PortalProfile>() },
        { 'W', GameManager.GetBoardEdgeItemProfile<WallProfile>() }
    };

    public static char GetSymbol(BoardEdgeItemProfile profile)
    {
        return _symbols.TryGetValue(profile, out char symbol) ? symbol : '?';
    }

    public static BoardEdgeItemProfile GetProfileFromSymbol(Char c)
    {
        if (!Char.IsLetter(c))
        {
            return null;
        }

        return _typesFromSymbol.TryGetValue(c, out BoardEdgeItemProfile profile) ? profile : null;
    }
}
