using Godot;

public partial class Torpedo : CharacterBody2D
{
	public Vector2 pos;
	public float rotation;
	public float direction;
	float speed = 300;
	public override void _Ready()
	{
		GlobalPosition = pos;
		GlobalRotation = rotation;
		base._Ready();
	}
	public void Initialize(Vector2 pos, float rotation){
		GlobalRotation = rotation;
		GlobalPosition = pos;
	}
	public override void _PhysicsProcess(double delta)
	{
		Velocity = new Vector2(0,-speed).Rotated(direction);
		MoveAndSlide();
	}
}
