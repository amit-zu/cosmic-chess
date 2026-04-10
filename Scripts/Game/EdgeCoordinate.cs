using Godot;

public partial class EdgeCoordinate : GodotObject
{
	public File File { get; }
	public Rank Rank { get; }
	public Direction Direction { get; }

	public EdgeCoordinate(File file, Rank rank, Direction direction)
	{
		File = file;
		Rank = rank;
		Direction = direction;
	}

	public bool Equals(EdgeCoordinate other)
	{
		return other != null && File == other.File && Rank == other.Rank && Direction == other.Direction;
	}

	public override string ToString()
	{
		return Util.GetEnumDescription(File) + (int)Rank + " " + Direction;
	}
}
