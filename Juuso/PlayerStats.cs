using Godot;
using System;

public partial class PlayerStats : Control
{
	[Export] private Submarine sub;
	[Export] private ProgressBar healthBar;

	public override void _Ready()
	{
		healthBar.Value = 100;
		sub.HealthCanged += HealthUpdate;
	}

	public void HealthUpdate(float hh){
		GD.Print(hh);
		healthBar.Value = hh;
	}
}
