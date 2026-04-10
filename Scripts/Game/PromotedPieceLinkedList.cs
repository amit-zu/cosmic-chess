using System;
using Godot;

public class PromotedPieceLinkedList
{
    private PieceNode Head { get; set; }

    public PromotedPieceLinkedList()
    {
        var queen = new PieceNode(GameManager.GetPieceProfile<QueenProfile>());
        var knight = new PieceNode(GameManager.GetPieceProfile<KnightProfile>());
        var rook = new PieceNode(GameManager.GetPieceProfile<RookProfile>());
        var bishop = new PieceNode(GameManager.GetPieceProfile<BishopProfile>());
        var archbishop = new PieceNode(GameManager.GetPieceProfile<ArchbishopProfile>());

        // Link forward
        queen.Next = knight;
        knight.Next = rook;
        rook.Next = bishop;
        bishop.Next = archbishop;
        archbishop.Next = queen; // Circular link

        // Link backward
        archbishop.Previous = bishop;
        bishop.Previous = rook;
        rook.Previous = knight;
        knight.Previous = queen;
        queen.Previous = archbishop; // Circular link

        Head = queen;
    }

    public void TraverseForward(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            Head = Head.Next;
        }
    }

    public void TraverseBackward(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            Head = Head.Previous;
        }
    }

    public void TraverseUntil(PieceProfile pieceProfile)
    {
        while (Head.PieceType != pieceProfile)
        {
            Head = Head.Next;
        }
    }

    public PieceProfile GetHeadOfList()
    {
        return Head.PieceType;
    }

    private class PieceNode
    {
        public PieceProfile PieceType;
        public Texture2D texture;
        public PieceNode Next;
        public PieceNode Previous;

        public PieceNode(PieceProfile pieceProfile)
        {
            PieceType = pieceProfile;
            Next = null;
            Previous = null;
        }
    }
}