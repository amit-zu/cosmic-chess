using System;
using System.Collections.Generic;
using System.Linq;

public class EnPassantContext
{
    public int Ply;
    public SquareCoordinate Destination;
    public List<SquareCoordinate> EligibleSquares;
    public SquareCoordinate CapturedPawnCoordinate;

    public EnPassantContext()
    {
        Ply = -1;
        Destination = null;
        EligibleSquares = new List<SquareCoordinate>();
        CapturedPawnCoordinate = null;
    }

    public EnPassantContext(int Ply, SquareCoordinate Destination, List<SquareCoordinate> EligibleSquares, SquareCoordinate CapturedPawnCoordinate)
    {
        this.Ply = Ply;
        this.Destination = Destination;
        this.EligibleSquares = EligibleSquares;
        this.CapturedPawnCoordinate = CapturedPawnCoordinate;
    }

    public override string ToString()
    {
        return Destination != null ? Destination.ToString() : "-";
    }
}