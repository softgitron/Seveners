using Godot;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public partial class Submarine : CharacterBody2D
{

	[Export]
	public float Weight = 100;
	[Export]
	public float WaterResistance = 0.9f;
	[Export]
	public float CurrentPropulsionForce = 0.0f;
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
	public float DefaultSteerAngle = 0.01f;
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
	private Vector2 zero = new Vector2(0, 0);

	private float health = 100;
	public override void _Ready()
	{
		_currentDirection = new Vector2(0, -1);
		aboveMaterial = (ShaderMaterial)aboveWater.Material;
		NodeCollection.Instance.RegisterNode(this);
	}

	[Signal] public delegate void HealthCangedEventHandler(float newHealth);
	[Signal] public delegate void GearCangedEventHandler(int metalGear);
	public void TakeDamage(float damage)
	{
		health -= damage;
		EmitSignal(SignalName.HealthCanged, health);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		// Update movement params:
		CurrentPropulsionForce = Math.Sign(CurrentGear) * CurrentRpm;
		//Debug.Print("CurrentPropulsionForce: " + CurrentPropulsionForce.ToString());
		Vector2 currentPropulsionVector = _currentDirection * CurrentPropulsionForce;
		Vector2 interpolatedResistedVelocity = Velocity.Lerp(currentPropulsionVector, (float)delta * WaterResistance);
		//Debug.Print("currentPropulsionVector: " + currentPropulsionVector.ToString());
		//Debug.Print("interpolatedResistedVelocity: " + interpolatedResistedVelocity.ToString());

		Velocity = interpolatedResistedVelocity;
		CurrentSpeed = Velocity.Length();
		//Debug.Print("Vel: " + Velocity.ToString());
		Rotation = -_currentDirection.AngleTo(up);

		// Using MoveAndCollide.
		var collision = MoveAndCollide(Velocity * (float)delta);
		if (collision != null)
		{
			GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
			TakeDamage(5);
			Velocity = -Velocity * 0.5f;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void MoveToDepth(float depth)
	{
		Depth = depth;
		isAboveWater = !isAboveWater;
		aboveMaterial.SetShaderParameter("isUnderWater", !isAboveWater);
		aboveWater.CollisionEnabled = isAboveWater;
		belowWater.CollisionEnabled = !isAboveWater; // This causes lag spike! TODO: Maybe better approach would be to change what colliders player reacts to (so no hiding colliders)
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Visible = isAboveWater;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Steer(int direction)
	{
		float degrees = (float)(direction * (DefaultSteerAngle * (float)(1 / Math.Pow(Math.E, 0.01 * CurrentSpeed))));
		//Debug.Print("Degrees: " + degrees.ToString());

		//Debug.Print("Turn factor: " + (1 / Math.Pow(Math.E, 0.01*CurrentSpeed)).ToString());
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
		EmitSignal(SignalName.GearCanged, CurrentGear);

	}

	public void Reset()
	{
		CurrentGear = 1;
		CurrentRpm = 0;
		Velocity = Vector2.Zero;
	}

	private void _on_timer_timeout()
	{
		//Debug.Print("ShipNode timer timed out");
	}

	private void HandleAudio()
	{

	}

	public override void _Notification(int what)
	{
		if (what == NotificationExitTree) NodeCollection.Instance.UnregisterNode(this);
	}

}
