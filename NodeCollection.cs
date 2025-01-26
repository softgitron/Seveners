using Godot;
using System;
using Godot.Collections;

public partial class NodeCollection : Node
{

	public const string RadarLocationRegisteredName = "RadarLocationRegistered";

	public static NodeCollection Instance { get; private set; }

	public Dictionary<ulong, Node2D> NodesToTrack { get; private set; } = new Dictionary<ulong, Node2D>();
	public Dictionary<ulong, ulong> NodesToUnRegister { get; private set; } = new Dictionary<ulong, ulong>();


	public override void _Ready()
	{
		Instance = this;
	}

	public void RegisterNode(Node2D node)
	{
		NodesToTrack.Add(node.GetInstanceId(), node);
	}

	/// <summary>
	/// NOTE: Does not work with multiple Node handlers since one needs to unregister the node :D Work with radar for now.
	/// </summary>
	/// <param name="node"></param>
	public void UnregisterNode(Node2D node) 
	{
		ulong nodeId = node.GetInstanceId();
		NodesToTrack.Remove(nodeId);
		// Todo this must be emptied somehow.
		NodesToUnRegister.Add(nodeId, nodeId);
	}

}
