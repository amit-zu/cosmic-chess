using Godot;
using System;

public partial class MainMenu : Control
{

	public override void _Ready()
	{
		PopupPanel settingsMenu = GetNode<PopupPanel>("SettingsPopup");
		settingsMenu.PopupHide += UnhideAllButtons;
	}

	public void HideAllButtons()
	{
		GetNode<OptionButton>("VariantSelect").Visible = false;
		GetNode<VBoxContainer>("ButtonsContainer").Visible = false;
	}

	public void UnhideAllButtons()
	{
		GetNode<OptionButton>("VariantSelect").Visible = true;
		GetNode<VBoxContainer>("ButtonsContainer").Visible = true;
	}

	private void OnPlayButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/game_screen.tscn");
	}

	private void OnLearnButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/level_select_screen.tscn");
	}

	private void OnSettingsButtonPressed()
	{
		PopupPanel settingsMenu = GetNode<PopupPanel>("SettingsPopup");
		settingsMenu.PopupCentered();
		settingsMenu.Visible = true;
		HideAllButtons();
	}
}
