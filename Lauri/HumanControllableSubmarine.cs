using Godot;
using System;
using System.Diagnostics;

public partial class HumanControllableSubmarine : Submarine
{
	public override void _PhysicsProcess(double delta)
	{
		_HandleInput(delta);
		base._PhysicsProcess(delta);
	}

	private void _HandleInput(double delta)
	{
		// Switching above/below water
		bool switchLayer = Input.IsActionJustPressed("surface");
		if (switchLayer)
		{
			float depthToMoveTo = Convert.ToInt32(isAboveWater) * 500.0f;
			base.MoveToDepth(depthToMoveTo);
		}

		int powerUp = Convert.ToInt32(Input.IsActionJustPressed("gear_up"));
		int powerDown = -Convert.ToInt32(Input.IsActionJustPressed("gear_down"));
		base.ChangeGear(powerUp + powerDown);

		int throttleUp = Convert.ToInt32(Input.IsActionPressed("throttle_up"));
		int throttleDown = -Convert.ToInt32(Input.IsActionPressed("throttle_down"));

		base.ChangeThrottle(throttleUp + throttleDown);

		float steerDegrees = (Convert.ToInt32(Input.IsActionPressed("steer_right")) * -SteerAngle) + (Convert.ToInt32(Input.IsActionPressed("steer_left")) * SteerAngle);
		base.Steer(steerDegrees);
	}

	private void _on_timer_timeout()
	{
		Debug.Print("ShipNode timer timed out");
	}

	private void HandleAudio()
	{

	}

}
