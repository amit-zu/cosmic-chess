using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

using static GameManager;

public partial class Util : GodotObject
{
	public static readonly Dictionary<Direction, Godot.Vector2> DirectionToVector = new Dictionary<Direction, Godot.Vector2>
	{
		{ Direction.Up, new Godot.Vector2(0, 1) },
		{ Direction.Down, new Godot.Vector2(0, -1) },
		{ Direction.Left, new Godot.Vector2(-1, 0) },
		{ Direction.Right, new Godot.Vector2(1, 0) },
		{ Direction.TopLeft, new Godot.Vector2(-1, 1) },
		{ Direction.TopRight, new Godot.Vector2(1, 1) },
		{ Direction.BottomLeft, new Godot.Vector2(-1, -1) },
		{ Direction.BottomRight, new Godot.Vector2(1, -1) }
	};

	public static string GetEnumDescription(Enum value)
	{
		var field = value.GetType().GetField(value.ToString());
		var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
		return attribute == null ? value.ToString() : attribute.Description;
	}

	public static Color GetOppositeColor(Color? color)
	{
		return color == Color.WHITE ? Color.BLACK : Color.WHITE;
	}

	public static void PrintSquares(List<SquareCoordinate> list)
	{
		list.ForEach(square => GD.Print(square.File.ToString() + square.Rank.ToString()));
		if (list.Count == 0)
		{
			GD.Print("Empty list.");
		}
	}

	public static List<SquareCoordinate> GetSquaresBetweenIncluding(SquareCoordinate square1, SquareCoordinate square2)
	{
		List<SquareCoordinate> squaresBetween = new List<SquareCoordinate>();

		int fileDiff = (int)square2.File - (int)square1.File;
		int rankDiff = (int)square2.Rank - (int)square1.Rank;

		if (fileDiff != 0 && rankDiff != 0 && Math.Abs(fileDiff) != Math.Abs(rankDiff))
		{
			return squaresBetween;
		}

		int fileStep = fileDiff == 0 ? 0 : (fileDiff > 0 ? 1 : -1);
		int rankStep = rankDiff == 0 ? 0 : (rankDiff > 0 ? 1 : -1);

		File file = square1.File;
		Rank rank = square1.Rank;

		while (true)
		{
			squaresBetween.Add(GetSquareCoordinate(file, rank));

			if (file == square2.File && rank == square2.Rank)
				break;

			file = (File)((int)file + fileStep);
			rank = (Rank)((int)rank + rankStep);
		}

		return squaresBetween;
	}

	public static bool ContainsSquare(List<SquareCoordinate> list, SquareCoordinate square)
	{
		if (square == null || list == null)
		{
			return false;
		}

		return list.Any(listSquare => listSquare.Equals(square));
	}

	public static Node FindChild(Node root, string name)
	{
		if (root.Name == name)
			return root;

		foreach (Node child in root.GetChildren())
		{
			var result = FindChild(child, name);
			if (result != null)
				return result;
		}
		return null;
	}

	public static T FindChild<T>(Node root) where T : Node
	{
		if (root is T match)
			return match;

		foreach (Node child in root.GetChildren())
		{
			var result = FindChild<T>(child);
			if (result != null)
				return result;
		}
		return null;
	}

	public static Direction GetDirectionFromVector(Godot.Vector2 vector)
	{
		return DirectionToVector.FirstOrDefault(x => x.Value == vector).Key;
	}

	public static Godot.Vector2 GetVectorFromDirection(Direction direction)
	{
		return DirectionToVector[direction];
	}

	public static Direction GetOppositeDirection(Direction direction)
	{
		return DirectionToVector.FirstOrDefault(x => x.Value == -DirectionToVector[direction]).Key;
	}

	public static File GetFileFromChar(char fileChar)
	{
		foreach (File file in Enum.GetValues(typeof(File)))
		{
			if (GetEnumDescription(file)[0] == fileChar.ToString().ToLower()[0])
			{
				return file;
			}
		}
		return File.A;
	}

	public static Rank GetRankFromChar(char rankChar)
	{
		foreach (Rank rank in Enum.GetValues(typeof(Rank)))
		{
			if ((int)rank == (int)char.GetNumericValue(rankChar))
			{
				return rank;
			}
		}

		return Rank.ONE;
	}

	public static SquareCoordinate GetCoordinateFromString(string coordString)
	{
		if (coordString.Length != 2)
		{
			GD.Print("Error: Invalid coordinate string.");
		}

		File file = GetFileFromChar(coordString[0]);
		Rank rank = GetRankFromChar(coordString[1]);

		return GetSquareCoordinate(file, rank);
	}

	public static bool IsSignalConnected(Node sender, string signalName, GodotObject target, string methodName)
	{
		var connections = sender.GetSignalConnectionList(signalName);

		foreach (var conn in connections)
		{
			var callable = (Callable)conn["callable"];

			if (callable.Target == target && callable.Method == methodName)
			{
				return true;
			}
		}

		return false;
	}
}
