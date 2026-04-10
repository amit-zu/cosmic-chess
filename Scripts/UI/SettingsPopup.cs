using Godot;
using System;

public partial class SettingsPopup : PopupPanel
{

	public override void _Ready()
	{
		GetNode<Button>("PopupControl/CloseAndSaveButtons/CloseButton").Pressed += CloseSettingsMenu;
		GetNode<Button>("PopupControl/CloseAndSaveButtons/SaveButton").Pressed += SaveSettings;
	}

	public override void _Process(double delta)
	{
		// TODO change settings labels according to slider values
		base._Process(delta);
	}

	private void CloseSettingsMenu()
	{
		Hide();
	}

	private void SaveSettings()
	{
		Hide();
	}
}
