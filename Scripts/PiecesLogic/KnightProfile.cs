using System.Collections.Generic;

using static GameManager;

public partial class KnightProfile : PieceProfile
{
    public KnightProfile()
    {
        Points = 3;
    }

    public override List<SquareCoordinate> GetLegalSquares(BoardState boardState, SquareCoordinate currentSquare, bool hasMoved)
    {
        List<SquareCoordinate> legalSquares = new List<SquareCoordinate>();
        Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

        if (currentSquare.File <= File.F && currentSquare.Rank <= Rank.SEVEN)
        {
            var move1 = GetSquareCoordinate(currentSquare.File + 2, currentSquare.Rank + 1);

            if (boardState.GetBoardItemAt(move1) == null)
            {
                legalSquares.Add(move1);
            }
        }

        if (currentSquare.File <= File.F && currentSquare.Rank >= Rank.TWO)
        {
            var move2 = GetSquareCoordinate(currentSquare.File + 2, currentSquare.Rank - 1);

            if (boardState.GetBoardItemAt(move2) == null)
            {
                legalSquares.Add(move2);
            }
        }

        if (currentSquare.File >= File.C && currentSquare.Rank <= Rank.SEVEN)
        {
            var move3 = GetSquareCoordinate(currentSquare.File - 2, currentSquare.Rank + 1);

            if (boardState.GetBoardItemAt(move3) == null)
            {
                legalSquares.Add(move3);
            }
        }

        if (currentSquare.File >= File.C && currentSquare.Rank >= Rank.TWO)
        {
            var move4 = GetSquareCoordinate(currentSquare.File - 2, currentSquare.Rank - 1);

            if (boardState.GetBoardItemAt(move4) == null)
            {
                legalSquares.Add(move4);
            }
        }

        if (currentSquare.File <= File.G && currentSquare.Rank <= Rank.SIX)
        {
            var move5 = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank + 2);

            if (boardState.GetBoardItemAt(move5) == null)
            {
                legalSquares.Add(move5);
            }
        }

        if (currentSquare.File >= File.B && currentSquare.Rank <= Rank.SIX)
        {
            var move6 = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank + 2);

            if (boardState.GetBoardItemAt(move6) == null)
            {
                legalSquares.Add(move6);
            }
        }

        if (currentSquare.File <= File.G && currentSquare.Rank >= Rank.THREE)
        {
            var move7 = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank - 2);

            if (boardState.GetBoardItemAt(move7) == null)
            {
                legalSquares.Add(move7);
            }
        }

        if (currentSquare.File >= File.B && currentSquare.Rank >= Rank.THREE)
        {
            var move8 = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank - 2);

            if (boardState.GetBoardItemAt(move8) == null)
            {
                legalSquares.Add(move8);
            }
        }

        return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), legalSquares);
    }

    public override List<SquareCoordinate> GetTakingSquares(BoardState boardState, SquareCoordinate currentSquare)
    {
        List<SquareCoordinate> takingSquares = new List<SquareCoordinate>();
        Color color = boardState.GetBoardItemAt(currentSquare).Value.Item2;

        if (currentSquare.File <= File.F && currentSquare.Rank <= Rank.SEVEN)
        {
            var move1 = GetSquareCoordinate(currentSquare.File + 2, currentSquare.Rank + 1);
            (PieceProfile, Color)? square = boardState.GetBoardItemAt(move1);

            if (square.HasValue && square.Value.Item1 != GameManager.GetPieceProfile<KingProfile>() && square.Value.Item2 != color)
            {
                takingSquares.Add(move1);
            }
        }

        if (currentSquare.File <= File.F && currentSquare.Rank >= Rank.TWO)
        {
            var move2 = GetSquareCoordinate(currentSquare.File + 2, currentSquare.Rank - 1);
            (PieceProfile, Color)? square = boardState.GetBoardItemAt(move2);

            if (square.HasValue && square.Value.Item1 != GameManager.GetPieceProfile<KingProfile>() && square.Value.Item2 != color)
            {
                takingSquares.Add(move2);
            }
        }

        if (currentSquare.File >= File.C && currentSquare.Rank <= Rank.SEVEN)
        {
            var move3 = GetSquareCoordinate(currentSquare.File - 2, currentSquare.Rank + 1);
            (PieceProfile, Color)? square = boardState.GetBoardItemAt(move3);

            if (square.HasValue && square.Value.Item1 != GameManager.GetPieceProfile<KingProfile>() && square.Value.Item2 != color)
            {
                takingSquares.Add(move3);
            }
        }

        if (currentSquare.File >= File.C && currentSquare.Rank >= Rank.TWO)
        {
            var move4 = GetSquareCoordinate(currentSquare.File - 2, currentSquare.Rank - 1);
            (PieceProfile, Color)? square = boardState.GetBoardItemAt(move4);

            if (square.HasValue && square.Value.Item1 != GameManager.GetPieceProfile<KingProfile>() && square.Value.Item2 != color)
            {
                takingSquares.Add(move4);
            }
        }

        if (currentSquare.File <= File.G && currentSquare.Rank <= Rank.SIX)
        {
            var move5 = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank + 2);
            (PieceProfile, Color)? square = boardState.GetBoardItemAt(move5);

            if (square.HasValue && square.Value.Item1 != GameManager.GetPieceProfile<KingProfile>() && square.Value.Item2 != color)
            {
                takingSquares.Add(move5);
            }
        }

        if (currentSquare.File >= File.B && currentSquare.Rank <= Rank.SIX)
        {
            var move6 = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank + 2);
            (PieceProfile, Color)? square = boardState.GetBoardItemAt(move6);

            if (square.HasValue && square.Value.Item1 != GameManager.GetPieceProfile<KingProfile>() && square.Value.Item2 != color)
            {
                takingSquares.Add(move6);
            }
        }

        if (currentSquare.File <= File.G && currentSquare.Rank >= Rank.THREE)
        {
            var move7 = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank - 2);
            (PieceProfile, Color)? square = boardState.GetBoardItemAt(move7);

            if (square.HasValue && square.Value.Item1 != GameManager.GetPieceProfile<KingProfile>() && square.Value.Item2 != color)
            {
                takingSquares.Add(move7);
            }
        }

        if (currentSquare.File >= File.B && currentSquare.Rank >= Rank.THREE)
        {
            var move8 = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank - 2);
            (PieceProfile, Color)? square = boardState.GetBoardItemAt(move8);

            if (square.HasValue && square.Value.Item1 != GameManager.GetPieceProfile<KingProfile>() && square.Value.Item2 != color)
            {
                takingSquares.Add(move8);
            }
        }

        return FilterLegalSquaresForCheck(boardState, currentSquare, (this, color), takingSquares);
    }

    public override List<SquareCoordinate> GetCheckingSquares(BoardState boardState, SquareCoordinate currentSquare)
    {
        List<SquareCoordinate> checkingSquares = new List<SquareCoordinate>();

        if (currentSquare.File <= File.F && currentSquare.Rank <= Rank.SEVEN)
        {
            var move1 = GetSquareCoordinate(currentSquare.File + 2, currentSquare.Rank + 1);

            checkingSquares.Add(move1);
        }

        if (currentSquare.File <= File.F && currentSquare.Rank >= Rank.TWO)
        {
            var move2 = GetSquareCoordinate(currentSquare.File + 2, currentSquare.Rank - 1);

            checkingSquares.Add(move2);
        }

        if (currentSquare.File >= File.C && currentSquare.Rank <= Rank.SEVEN)
        {
            var move3 = GetSquareCoordinate(currentSquare.File - 2, currentSquare.Rank + 1);

            checkingSquares.Add(move3);
        }

        if (currentSquare.File >= File.C && currentSquare.Rank >= Rank.TWO)
        {
            var move4 = GetSquareCoordinate(currentSquare.File - 2, currentSquare.Rank - 1);

            checkingSquares.Add(move4);
        }

        if (currentSquare.File <= File.G && currentSquare.Rank <= Rank.SIX)
        {
            var move5 = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank + 2);

            checkingSquares.Add(move5);
        }

        if (currentSquare.File >= File.B && currentSquare.Rank <= Rank.SIX)
        {
            var move6 = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank + 2);

            checkingSquares.Add(move6);
        }

        if (currentSquare.File <= File.G && currentSquare.Rank >= Rank.THREE)
        {
            var move7 = GetSquareCoordinate(currentSquare.File + 1, currentSquare.Rank - 2);

            checkingSquares.Add(move7);
        }

        if (currentSquare.File >= File.B && currentSquare.Rank >= Rank.THREE)
        {
            var move8 = GetSquareCoordinate(currentSquare.File - 1, currentSquare.Rank - 2);

            checkingSquares.Add(move8);
        }

        return checkingSquares;
    }

    public override string ToString()
    {
        return "Knight";
    }
}