using Godot;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public partial class Submarine : CharacterBody2D
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
	// More depth -> down
	[Export]
	public float Depth = 0.0f;
	[Export]
	public float MaxDepth = 1000.0f;
	[Export]
	public float MinDepth = 0.0f;

	[Export] TileMapLayer aboveWater;
	[Export] TileMapLayer belowWater;
	private ShaderMaterial aboveMaterial;

	protected bool isAboveWater = true;
	protected Vector2 _currentDirection;

	protected Vector2 up = new Vector2(0, -1);
	
	public override void _Ready()
	{
		_currentDirection = new Vector2(0, -1);
		aboveMaterial = (ShaderMaterial)aboveWater.Material;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		// Update movement params:
		CurrentSpeed = CurrentRpm;
		Velocity = _currentDirection * CurrentSpeed;
		Rotation = -_currentDirection.AngleTo(up);

		// Using MoveAndCollide.
		var collision = MoveAndCollide(Velocity * (float)delta);
		if (collision != null)
		{
			GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void MoveToDepth(float depth)
	{
		isAboveWater = !isAboveWater;
		aboveMaterial.SetShaderParameter("isUnderWater", !isAboveWater);
		belowWater.CollisionEnabled = !isAboveWater; // This causes lag spike! TODO: Maybe better approach would be to change what colliders player reacts to (so no hiding colliders)
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Steer(float degrees)
	{
		_currentDirection = _currentDirection.Rotated(-degrees);
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ChangeThrottle(int throttleChange)
	{
		Debug.Assert(-1 <= throttleChange && throttleChange <= 1, "throttleChange should be between 1 and -1!");
		CurrentRpm = Math.Max(Math.Min(CurrentRpm + (throttleChange * ThrottleShiftInterval), CurrentMaxRpm), MinRpm);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ChangePower(int powerChange)
	{
		Debug.Assert(-1 <= powerChange && powerChange <= 1, "powerChange should be between 1 and -1!");
		CurrentMaxRpm = Math.Max(Math.Min(CurrentMaxRpm + (powerChange*RpmLimiterShiftInterval), MaxRpm), MinRpm);
	}

	private void _on_timer_timeout()
	{
		Debug.Print("ShipNode timer timed out");
	}

	private void HandleAudio()
	{

	}

}
