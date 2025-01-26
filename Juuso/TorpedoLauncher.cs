using Godot;
using System;

public partial class TorpedoLauncher : Node2D
{
    [Export] Timer shootTimer;
    private bool canShoot = true;

	PackedScene bulletScene = (PackedScene)GD.Load("res://Juuso/Torpedo.tscn");
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("shoot")){
            if (canShoot){
			    Fire();
                shootTimer.Start();
                canShoot = false;
            }
		}
	}

    public void _on_player_shoot_timer_timeout(){
canShoot = true;
    }

	public void Fire(){
		Torpedo torpedo = (Torpedo)bulletScene.Instantiate();
		torpedo.pos = GlobalPosition;
		torpedo.direction = GlobalRotation;
		torpedo.rotation = GlobalRotation;
		GetTree().Root.AddChild(torpedo);
	}
}
