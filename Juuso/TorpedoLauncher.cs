using Godot;
using System;

public partial class TorpedoLauncher : Node2D
{
	PackedScene bulletScene = (PackedScene)GD.Load("res://Juuso/Torpedo.tscn");
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("shoot")){
			GD.Print("Shoow");
			Fire();
		}
	}

	public void Fire(){
		Torpedo torpedo = (Torpedo)bulletScene.Instantiate();
		torpedo.pos = GlobalPosition;
		torpedo.direction = GlobalRotation;
		torpedo.rotation = GlobalRotation;
		GetTree().Root.AddChild(torpedo);
	}
}
