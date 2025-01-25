using Godot;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;

public partial class ShipNode : CharacterBody2D
{
	
	[Export]
	public double Weight = 100;
	[Export]
	public float Speed = 100.0f;
	[Export]
	public float CurrentPower = 0.0f;
	[Export]
	public float TargetPower = 100.0f;
	[Export]
	public float PowerShiftInterval = 20.0f;
	[Export]
	public float MaxPower = 100.0f;
	[Export]
	public float MinPower = 100.0f;
	[Export]
	public float SteerPower = 0.1f;
	[Export]
	public float SteerAngle = 360/(2*float.Pi);

	private Vector2 _currentDirection; 
	
	public override void _Ready()
	{
		_currentDirection = new Vector2(1, 0);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		_HandleInput();

		// Using MoveAndCollide.
		var collision = MoveAndCollide(Velocity * (float)delta);
		if (collision != null)
		{
			GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
		}
	}

	private void _HandleInput()
	{
		Debug.Print("Handling input");
		if (Input.IsActionPressed("throttle"))
		{
			Debug.Print("Throttle pressed");
			Speed = 10.0f;
		}
		
		if (Input.IsActionPressed("steer_left"))
		{
			Debug.Print("steer left pressed");
			_currentDirection = _currentDirection.Rotated(SteerAngle);
		}
		
		if (Input.IsActionPressed("steer_right"))
		{
			Debug.Print("steer right pressed");
			_currentDirection = _currentDirection.Rotated(-SteerAngle);
		}

		if (Input.IsActionPressed("reverse"))
		{
			Debug.Print("reverse pressed");
			Speed = -10.0f;
		}

		bool powerUp = Input.IsActionJustPressed("power_up");
		bool powerDown = Input.IsActionJustPressed("power_down");

		if (powerUp && powerDown)
		{
			// Nothing
		} else if (powerUp)
		{
			CurrentPower += PowerShiftInterval;
			CurrentPower = Math.Min(CurrentPower, MaxPower);
		} else if (powerDown)
		{
			CurrentPower -= PowerShiftInterval;
			CurrentPower = Math.Max(CurrentPower, MinPower);
		}

		Velocity = _currentDirection * CurrentPower;
	}

	private void _on_timer_timeout()
	{
		Debug.Print("ShipNode timer timed out");
	}

	private void HandleAudio()
	{

	}

}
