using Godot;
using System;

public partial class LevelSelectScreen : Control
{
	public void GoToLevel1()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Levels/level_1.tscn");
	}

	public void GoToLevel2()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Levels/level_2.tscn");
	}

	public void GoToLevel3()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Levels/level_3.tscn");
	}
}
