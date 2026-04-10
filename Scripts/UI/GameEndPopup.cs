using Godot;
using System;

public partial class GameEndPopup : CanvasLayer
{
	public override void _Ready()
	{
		Button mainMenuButton = GetNode<Button>("Control/Panel/VBoxContainer/MainMenuButton");
		mainMenuButton.Pressed += ReturnToMainMenu;
	}

	public void ReturnToMainMenu()
	{
		GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
	}
}
