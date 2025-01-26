using Godot;
using System;

public partial class PlayerStats : Control
{
	[Export] private Submarine sub;
	[Export] private MainGameLogic gameLogic;

	[Export] private ProgressBar healthBar;
	[Export] private Label gearText;

	[Export] private Label levelText;

	public override void _Ready()
	{
		healthBar.Value = 100;
		sub.HealthCanged += HealthUpdate;
		sub.GearCanged += GearUpdate;
		gameLogic.LevelCanged += LevelUpdate;
	}

	private void LevelUpdate(int level)
	{
		levelText.Text = $"Level {level}";
	}

	private void GearUpdate(int metalGear)
	{
		gearText.Text = $"Gear: {metalGear}";
	}

	public void HealthUpdate(float hh){
		GD.Print(hh);
		healthBar.Value = hh;
	}
}
