public class Move
{
    public PieceProfile PieceProfile;
    public SquareCoordinate OriginSquare;
    public SquareCoordinate DestinationSquare;
    public Color Color;
    public PieceProfile? CapturedPieceProfile; // Null if no piece was captured
    public PieceProfile? PromotionPieceProfile; // Null if not a promotion
    public CastlingType? CastlingType; // Null if not a castling move
    public bool IsEnPassant; // True if the move is an en passant capture

    public Move(PieceProfile pieceProfile, SquareCoordinate originSquare, SquareCoordinate destinationSquare, Color color, PieceProfile? capturedPieceProfile = null, PieceProfile? promotionPieceProfile = null, CastlingType? castlingType = null, bool isEnPassant = false)
    {
        PieceProfile = pieceProfile;
        OriginSquare = originSquare;
        DestinationSquare = destinationSquare;
        Color = color;
        CapturedPieceProfile = capturedPieceProfile;
        PromotionPieceProfile = promotionPieceProfile;
        CastlingType = castlingType;
        IsEnPassant = isEnPassant;
    }

    public override string ToString()
    {
        return $"{PieceProfile} from {OriginSquare.File}{OriginSquare.Rank} to {DestinationSquare.File}{DestinationSquare.Rank}" +
               $"{(CapturedPieceProfile != null ? $", capturing {CapturedPieceProfile}" : "")}" +
               $"{(PromotionPieceProfile != null ? $", promoting to {PromotionPieceProfile}" : "")}" +
               $"{(CastlingType != null ? $", castling {CastlingType}" : "")}" +
               $"{(IsEnPassant ? ", en passant" : "")}";
    }
}