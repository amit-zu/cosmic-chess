using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

using static GameManager;

public abstract partial class PieceProfile : GodotObject
{
	public int Points;
	public abstract List<SquareCoordinate> GetLegalSquares(BoardState boardState, SquareCoordinate currentSquare, bool hasMoved);
	public abstract List<SquareCoordinate> GetTakingSquares(BoardState boardState, SquareCoordinate currentSquare);
	public abstract List<SquareCoordinate> GetCheckingSquares(BoardState boardState, SquareCoordinate currentSquare);

	public static List<SquareCoordinate> FilterLegalSquaresForCheck(BoardState boardState, SquareCoordinate initialSquare, (PieceProfile, Color) boardItem, List<SquareCoordinate> squares)
	{
		return squares.Where(square => !boardState.GetBoardStateAfterMove(boardItem, initialSquare, square).WhoIsInCheck.Contains(boardItem.Item2)).ToList();
	}

	public static explicit operator PieceProfile(Type v)
	{
		throw new NotImplementedException();
	}

	protected static bool OnBoardEdge(SquareCoordinate squareCoordinate)
	{
		return squareCoordinate.File == File.A || squareCoordinate.File == File.H || squareCoordinate.Rank == Rank.ONE || squareCoordinate.Rank == Rank.EIGHT;
	}

	protected void TraverseInDirection(File startingFile, Rank startingRank, BoardState boardState, List<SquareCoordinate> squares, Vector2 movementVector, int bouncesCount = 0)
	{
		SquareCoordinate tempSquare = GetSquareCoordinate(startingFile, startingRank);
		int teleportsLeft = 1;
		bool transformed;

		while (true)
		{
			transformed = false;

			if (OnBoardEdge(tempSquare))
			{
				Direction[] cardinalDirections = GetCardinalDirections(movementVector);

				(EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)? boardEdgeItemProfile1 = boardState.GetBoardEdgeItemAt(GetEdgeCoordinate(tempSquare.File, tempSquare.Rank, cardinalDirections[0]));
				(EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)? boardEdgeItemProfile2 = boardState.GetBoardEdgeItemAt(GetEdgeCoordinate(tempSquare.File, tempSquare.Rank, cardinalDirections[1]));

				(EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)? boardEdgeItemProfile = boardEdgeItemProfile1 ?? boardEdgeItemProfile2;

				if (boardEdgeItemProfile.HasValue)
				{
					EdgeCoordinate edgeItemCoordinate = boardEdgeItemProfile.Value.Item1;
					BoardEdgeItemProfile itemProfile = boardEdgeItemProfile.Value.Item2;
					EdgeCoordinate? connectedItemCoordinate = boardEdgeItemProfile.Value.Item3;

					if (itemProfile.GetType() == typeof(PortalProfile) && teleportsLeft > 0)
					{
						transformed = itemProfile.Transform(connectedItemCoordinate, Util.GetOppositeDirection(edgeItemCoordinate.Direction), ref tempSquare, ref movementVector, ref bouncesCount);


						if (transformed)
						{
							if (boardState.GetBoardItemAt(tempSquare) == null)
							{
								squares.Add(tempSquare);
							}
							teleportsLeft--;
						}
					}
				}
				else if (bouncesCount > 0)
				{

					if ((tempSquare.File == File.A && movementVector.X == -1) || (tempSquare.File == File.H && movementVector.X == 1))
					{
						movementVector.X = -movementVector.X;
						bouncesCount--;
					}
					if ((tempSquare.Rank == Rank.ONE && movementVector.Y == -1) || (tempSquare.Rank == Rank.EIGHT && movementVector.Y == 1))
					{
						movementVector.Y = -movementVector.Y;
						bouncesCount--;
					}
				}
			}
			if (!transformed)
			{
				tempSquare = GetSquareCoordinate((File)((int)tempSquare.File + (int)movementVector.X), (Rank)((int)tempSquare.Rank + (int)movementVector.Y));
				if (tempSquare == null)
				{
					break;
				}

				var boardItem = boardState.GetBoardItemAt(tempSquare);

				if (boardItem != null)
				{
					break;
				}
				else
				{
					squares.Add(tempSquare);
				}
			}
		}
	}

	protected void CaptureInDirection(File startingFile, Rank startingRank, Color color, BoardState boardState, List<SquareCoordinate> squares, Vector2 movementVector, int bouncesCount = 0)
	{
		SquareCoordinate tempSquare = GetSquareCoordinate(startingFile, startingRank);
		int teleportsLeft = 1;
		bool transformed;

		while (true)
		{
			transformed = false;

			if (OnBoardEdge(tempSquare))
			{
				Direction[] cardinalDirections = GetCardinalDirections(movementVector);

				(EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)? boardEdgeItemProfile1 = boardState.GetBoardEdgeItemAt(GetEdgeCoordinate(tempSquare.File, tempSquare.Rank, cardinalDirections[0]));
				(EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)? boardEdgeItemProfile2 = boardState.GetBoardEdgeItemAt(GetEdgeCoordinate(tempSquare.File, tempSquare.Rank, cardinalDirections[1]));

				(EdgeCoordinate, BoardEdgeItemProfile, EdgeCoordinate)? boardEdgeItemProfile = boardEdgeItemProfile1 ?? boardEdgeItemProfile2;

				if (boardEdgeItemProfile.HasValue)
				{
					EdgeCoordinate edgeItemCoordinate = boardEdgeItemProfile.Value.Item1;
					BoardEdgeItemProfile itemProfile = boardEdgeItemProfile.Value.Item2;
					EdgeCoordinate? connectedItemCoordinate = boardEdgeItemProfile.Value.Item3;

					if (itemProfile.GetType() == typeof(PortalProfile) && teleportsLeft > 0)
					{
						transformed = itemProfile.Transform(connectedItemCoordinate, Util.GetOppositeDirection(edgeItemCoordinate.Direction), ref tempSquare, ref movementVector, ref bouncesCount);

						if (transformed)
						{
							teleportsLeft--;
						}
					}
				}
				else if (bouncesCount > 0)
				{

					if ((tempSquare.File == File.A && movementVector.X == -1) || (tempSquare.File == File.H && movementVector.X == 1))
					{
						movementVector.X = -movementVector.X;
						bouncesCount--;
					}
					if ((tempSquare.Rank == Rank.ONE && movementVector.Y == -1) || (tempSquare.Rank == Rank.EIGHT && movementVector.Y == 1))
					{
						movementVector.Y = -movementVector.Y;
						bouncesCount--;
					}
				}
			}
			if (!transformed)
			{
				tempSquare = GetSquareCoordinate((File)((int)tempSquare.File + (int)movementVector.X), (Rank)((int)tempSquare.Rank + (int)movementVector.Y));
				if (tempSquare == null)
				{
					break;
				}

				var boardItem = boardState.GetBoardItemAt(tempSquare);

				if (boardItem != null)
				{
					if (boardItem.Value.Item2 == Util.GetOppositeColor(color)
					&& !(boardItem.Value.Item1 == GetPieceProfile<KingProfile>()))
					{
						squares.Add(tempSquare);
					}
					break;
				}
			}
		}
	}

	protected void CheckInDirection(File startingFile, Rank startingRank, Color color, BoardState boardState, List<SquareCoordinate> squares, Vector2 movementVector, int bouncesCount = 0)
	{
		SquareCoordinate tempSquare = GetSquareCoordinate(startingFile, startingRank);

		while (true)
		{
			if (OnBoardEdge(tempSquare))
			{
				if (bouncesCount > 0)
				{

					if ((tempSquare.File == File.A && movementVector.X == -1) || (tempSquare.File == File.H && movementVector.X == 1))
					{
						movementVector.X = -movementVector.X;
						bouncesCount--;
					}
					if ((tempSquare.Rank == Rank.ONE && movementVector.Y == -1) || (tempSquare.Rank == Rank.EIGHT && movementVector.Y == 1))
					{
						movementVector.Y = -movementVector.Y;
						bouncesCount--;
					}
				}
			}

			tempSquare = GetSquareCoordinate((File)((int)tempSquare.File + (int)movementVector.X), (Rank)((int)tempSquare.Rank + (int)movementVector.Y));
			if (tempSquare == null)
			{
				break;
			}

			var boardItem = boardState.GetBoardItemAt(tempSquare);

			if (boardItem == null || boardItem.Value.Item2 == Util.GetOppositeColor(color))
			{
				squares.Add(tempSquare);

				if (boardItem != null && !(boardItem.Value.Item1 == GetPieceProfile<KingProfile>()))
					break;
			}
			else if (boardItem.Value.Item2 == color)
			{
				squares.Add(tempSquare);
				break;
			}
		}
	}

	protected static Direction[] GetCardinalDirections(Vector2 movementVector)
	{
		Direction[] directions = [GetDirection(new Vector2(movementVector.X, 0)), GetDirection(new Vector2(0, movementVector.Y))];
		return directions;
	}

	protected static Direction GetDirection(Vector2 movementVector)
	{
		if (movementVector.X == 1 && movementVector.Y == 0)
			return Direction.Right;
		if (movementVector.X == -1 && movementVector.Y == 0)
			return Direction.Left;
		if (movementVector.X == 0 && movementVector.Y == 1)
			return Direction.Up;
		if (movementVector.X == 0 && movementVector.Y == -1)
			return Direction.Down;
		if (movementVector.X == 1 && movementVector.Y == 1)
			return Direction.TopRight;
		if (movementVector.X == -1 && movementVector.Y == 1)
			return Direction.TopLeft;
		if (movementVector.X == 1 && movementVector.Y == -1)
			return Direction.BottomRight;
		if (movementVector.X == -1 && movementVector.Y == -1)
			return Direction.BottomLeft;
		if (movementVector == Vector2.Zero)
			return Direction.None;

		throw new ArgumentException("Invalid movement vector for direction determination.");
	}
}
