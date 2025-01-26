using Godot;
using System;
using System.Diagnostics;

public partial class RadarBlip : AnimatedSprite2D
{
	
	public override void _Ready()
	{
		//SignalBus.Instance.RadarLocationRegistered += HandleRadarEntityLocation;
	}
	
}
