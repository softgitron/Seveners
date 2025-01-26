using Godot;
using System;

public partial class Goal : Area2D
{
	
	public override void _Ready()
	{
		base._Ready();
		NodeCollection.Instance.RegisterNode(this);
	}
	
	public override void _Notification(int what)
	{
		if (what == NotificationExitTree) NodeCollection.Instance.UnregisterNode(this);
	}
	
}
