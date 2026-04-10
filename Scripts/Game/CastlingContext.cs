public class CastlingContext
{
    public bool WhiteCanCastleKingside;
    public bool WhiteCanCastleQueenside;
    public bool BlackCanCastleKingside;
    public bool BlackCanCastleQueenside;

    public CastlingContext()
    {
        WhiteCanCastleKingside = true;
        WhiteCanCastleQueenside = true;
        BlackCanCastleKingside = true;
        BlackCanCastleQueenside = true;
    }

    public CastlingContext(bool WhiteCanCastleKingside, bool WhiteCanCastleQueenside, bool BlackCanCastleKingside, bool BlackCanCastleQueenside)
    {
        this.WhiteCanCastleKingside = WhiteCanCastleKingside;
        this.WhiteCanCastleQueenside = WhiteCanCastleQueenside;
        this.BlackCanCastleKingside = BlackCanCastleKingside;
        this.BlackCanCastleQueenside = BlackCanCastleQueenside;
    }

    public override string ToString()
    {
        string castlingRights = "";
        if (WhiteCanCastleKingside) castlingRights += "K";
        if (WhiteCanCastleQueenside) castlingRights += "Q";
        if (BlackCanCastleKingside) castlingRights += "k";
        if (BlackCanCastleQueenside) castlingRights += "q";
        if (castlingRights == "") castlingRights = "-";
        return castlingRights;
    }
}