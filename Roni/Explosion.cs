using Godot;
using System;

public partial class Explosion : Node2D
{
	public void _on_timer_timeout()
	{
		QueueFree();
	}
}
