using Godot;

public partial class SquareCoordinate : GodotObject
{
	public File File { get; }
	public Rank Rank { get; }

	public SquareCoordinate(File file, Rank rank)
	{
		File = file;
		Rank = rank;
	}

	public bool Equals(SquareCoordinate other)
	{
		return File == other.File && Rank == other.Rank;
	}

	public static SquareCoordinate FromString(string coordinate)
	{
		if (coordinate.Length != 2)
		{
			throw new System.ArgumentException("Invalid coordinate string");

		}

		File file = Util.GetFileFromChar(coordinate[0]);
		Rank rank = (Rank)char.GetNumericValue(coordinate[1]);

		return new SquareCoordinate(file, rank);
	}

	public override string ToString()
	{
		return Util.GetEnumDescription(File).ToLower() + (int)Rank;
	}
}
