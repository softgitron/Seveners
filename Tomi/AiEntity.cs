using Godot;
using System;

public partial class AiEntity : CharacterBody2D
{
	[Export]
	public float _movementSpeed = 1000f;
	[Export]
	public Marker2D _movementTarget;
	[Export]
	public NavigationAgent2D _navAgent;
	[Export]
	public Path2D _path;
    public override void _Ready()
    {
		CallDeferred("SetMovementTarget");
    }

	public void SetMovementTarget()
	{
		_navAgent.TargetPosition = _movementTarget.Position;


        //_path.Curve.AddPoint(ToLocal(this.GlobalPosition));
        //_path.Curve.AddPoint(ToLocal(_movementTarget.GlobalPosition));
    }
    public override void _PhysicsProcess(double delta)
	{
		if (_navAgent.IsNavigationFinished())
		{
			return;
		}
		Vector2 currentEntityPosition = this.GlobalPosition;
		Vector2 nextPathPosition = _navAgent.GetNextPathPosition();
		Vector2 newVelocity = nextPathPosition - currentEntityPosition;
		newVelocity = newVelocity.Normalized();
		newVelocity *= _movementSpeed * (float)delta;


        Velocity = newVelocity;
		MoveAndSlide();
	}
}
