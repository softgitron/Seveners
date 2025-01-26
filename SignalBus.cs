using Godot;
using System;

public partial class SignalBus : Node
{
	[Signal]
	public delegate void RadarLocationRegisteredEventHandler(Vector2 globalLocation);

	public const string RadarLocationRegisteredName = "RadarLocationRegistered";

	public static SignalBus Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
	}

}
