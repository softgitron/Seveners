using Godot;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;

public partial class ShipNode : Node2D
{
	
	[Export]
	public double Weight = 100;
	[Export]
	public float Speed = 500.0f;
    [Export]
    public float Power = 100.0f;
    [Export]
    public float PowerShiftInterval = 20.0f;

	private const float maxPower = 100.0f;
	
	public override void _Ready()
	{
		
	}
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
	}

	private void _HandleInput()
	{

		if (Input.IsActionPressed("throttle"))
		{

		}
		
		if (Input.IsActionPressed("steer_left"))
		{

		}
		
		if (Input.IsActionPressed("steer_right"))
		{

		}

        if (Input.IsActionPressed("reverse"))
        {

        }

		bool powerUp = Input.IsActionJustPressed("power_up");
		bool powerDown = Input.IsActionJustPressed("power_down");

		if (powerUp && powerDown)
		{
			// Nothing
		} else if (powerUp)
		{
			Power += PowerShiftInterval;
        } else if (powerDown)
        {
			Power -= PowerShiftInterval;
        }


    }

	private void _on_timer_timeout()
	{
		Debug.Print("ShipNode timer timed out");
	}

	private void HandleAudio()
	{

	}

}
