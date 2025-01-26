using Godot;
using System;

public partial class PlayerStats : Control
{
	[Export] private Submarine sub;
	[Export] private ProgressBar healthBar;
	[Export] private Label gearText;

	public override void _Ready()
	{
		healthBar.Value = 100;
		sub.HealthCanged += HealthUpdate;
		sub.GearCanged += GearUpdate;
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
