using Godot;

public partial class Torpedo : CharacterBody2D
{
	Vector2 pos;
	float rotation;
	float direction;
	float speed = 50;
	public override void _Ready()
	{
		GlobalPosition = pos;
		GlobalRotation = rotation;
		base._Ready();
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Velocity = new Vector2(speed,0).Rotated(direction);
		MoveAndSlide();
	}
}
