using Godot;
using System;
using System.Diagnostics;

public partial class ShipNode : CharacterBody2D
{

	[Export]
	public double Weight = 100;
	[Export]
	public float CurrentSpeed = 0.0f;
	[Export]
	public float CurrentRpm = 0.0f;
	[Export]
	public float CurrentTargetRpm = 0.0f;
	[Export]
	public float ThrottleShiftInterval = 1.0f;
	[Export]
	public float RpmLimiterShiftInterval = 10.0f;
	[Export]
	public float MaxRpm = 100.0f;
	[Export]
	public float CurrentMaxRpm = 10.0f;
	[Export]
	public float MinRpm = -10.0f;
	[Export]
	public float SteerPower = 0.1f;
	[Export]
	public float SteerAngle = 0.005f;
	[Export] TileMapLayer aboveWater;
	[Export] TileMapLayer belowWater;

	private Vector2 _currentDirection;

	private Vector2 up = new Vector2(0, -1);

	public override void _Ready()
	{
		_currentDirection = new Vector2(0, -1);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		_HandleInput(delta);

		// Using MoveAndCollide.
		var collision = MoveAndCollide(Velocity * (float)delta);
		if (collision != null)
		{
			GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
		}
	}

	private void _HandleInput(double delta)
	{
		bool switchLayer = Input.IsActionJustPressed("surface");

		if (switchLayer){
			aboveWater.Enabled = !aboveWater.Enabled;
			belowWater.CollisionEnabled = !aboveWater.Enabled;
		}
		bool powerUp = Input.IsActionJustPressed("power_up");
		bool powerDown = Input.IsActionJustPressed("power_down");

		if (powerUp && powerDown)
		{
			// Nothing
		}
		else if (powerUp)
		{
			CurrentMaxRpm = Math.Min(CurrentMaxRpm + RpmLimiterShiftInterval, MaxRpm);
		}
		else if (powerDown)
		{
			CurrentMaxRpm -= RpmLimiterShiftInterval;
			CurrentMaxRpm = Math.Max(CurrentMaxRpm - RpmLimiterShiftInterval, MinRpm);
		}

		if (Input.IsActionPressed("throttle_up"))
		{
			CurrentRpm = Math.Min(CurrentRpm + ThrottleShiftInterval, CurrentMaxRpm);
			//Debug.Print(CurrentRpm.ToString());
		}

		if (Input.IsActionPressed("throttle_down"))
		{
			CurrentRpm = Math.Max(CurrentRpm - ThrottleShiftInterval, MinRpm);
			//Debug.Print(CurrentRpm.ToString());
		}

		if (Input.IsActionPressed("steer_left"))
		{
			_currentDirection = _currentDirection.Rotated(-SteerAngle);
		}

		if (Input.IsActionPressed("steer_right"))
		{
			_currentDirection = _currentDirection.Rotated(SteerAngle);
		}

		CurrentSpeed = CurrentRpm;
		//Debug.Print(CurrentSpeed.ToString());

		Velocity = _currentDirection * CurrentSpeed;
		Rotation = -_currentDirection.AngleTo(up);
	}

	private void _on_timer_timeout()
	{
		Debug.Print("ShipNode timer timed out");
	}

	private void HandleAudio()
	{

	}

}
