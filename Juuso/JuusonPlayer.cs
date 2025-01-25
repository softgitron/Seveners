using Godot;
using System;

public partial class JuusonPlayer : CharacterBody2D
{
[Export] private float MAX_SPEED = 50;
[Export] private double ACCELERATION = 30;
[Export] private double FRICTION = 10;
[Export] private float RotationSpeed = 1.5f;


	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionPressed("ui_right")){
			Rotate(RotationSpeed * (float)delta);
		}
		if (Input.IsActionPressed("ui_left")){
			Rotate(-RotationSpeed * (float)delta);
		}
		Vector2 velocity = new Vector2();
		if (Input.IsActionPressed("ui_up")){
			velocity = Vector2.Up.Rotated(Rotation) * MAX_SPEED;
		}
		if (Input.IsActionPressed("ui_down")){
			velocity = Vector2.Down.Rotated(Rotation) * MAX_SPEED;
		}
		Velocity = velocity;
		MoveAndSlide();
	}

}
