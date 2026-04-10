using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node
{
	public Timer timer;
	[Export]
	public Game game;
	[Export]
	public Color playerColor;
	[Export]
	public Label timerLabel;
	public List<PieceProfile> CapturedPieces;

	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");

		timerLabel.Text = FormatTime(timer.TimeLeft);

		if (game.CurrentTurn != playerColor)
		{
			timer.Paused = true;
		}

		timer.Timeout += LostByTimeout;

		CapturedPieces = new List<PieceProfile>();
	}

	public override void _Process(double delta)
	{
		if (game.CurrentTurn == playerColor)
		{
			timerLabel.Text = FormatTime(timer.TimeLeft);
		}
	}

	public void StartClock()
	{
		timer.Paused = false;
	}

	public void StopClock()
	{
		timer.Paused = true;
	}

	public void LostByTimeout()
	{
		StopClock();
		game.Timeout(playerColor);
	}

	public void AddCapturedPiece(PieceProfile piece)
	{
		CapturedPieces.Add(piece);
	}

	private string FormatTime(double time)
	{
		int seconds = (int)time;
		int minutes = seconds / 60;
		seconds %= 60;
		return $"{minutes:D2}:{seconds:D2}";
	}
}
