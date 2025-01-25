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
	public float BaseMaxRpm = 20.0f;
	[Export]
	public float MaxRpm = 100.0f;
	[Export]
	public float CurrentMaxRpm = 10.0f;
	[Export]
	public float MinRpm = 0.0f;
	[Export]
	public float SteerAngle = 0.005f;
	// More depth -> down
	[Export]
	public float Depth = 0.0f;
	[Export]
	public float MaxDepth = 1000.0f;
	[Export]
	public float MinDepth = 0.0f;
	// -1 is reverse
	[Export]
	public int CurrentGear = 1;
	[Export]
	public int MaxGear = 5;
	private int MinGear = -1;

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
		CurrentSpeed = Math.Sign(CurrentGear) * CurrentRpm;

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
		Depth = depth;
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
		//Debug.Print("CurrentRpm changed to: " + CurrentRpm.ToString());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ChangeGear(int gearChange)
	{
		//Debug.Print("ShipNode timer timed out");
		Debug.Assert(-1 <= gearChange && gearChange <= 1, "gearChange should be between -1 and 1!");
		CurrentGear += gearChange;
		CurrentGear = Math.Max(Math.Min(CurrentGear, MaxGear), MinGear);
		//Debug.Print("CurrentGear changed to: " + CurrentGear.ToString());

		CurrentMaxRpm = Math.Max(Math.Min(BaseMaxRpm * Math.Abs(CurrentGear), MaxRpm), MinRpm);
		//Debug.Print("CurrentMaxRpm changed to: " + CurrentMaxRpm.ToString());
	}

	private void _on_timer_timeout()
	{
		//Debug.Print("ShipNode timer timed out");
	}

	private void HandleAudio()
	{

	}

}
