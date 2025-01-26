using Godot;
using Godot.Collections;
using System;

public partial class Radar : Sprite2D
{
	private Dictionary<ulong, RadarBlip> _NodeIdToBlip;
	PackedScene blipScene = (PackedScene)GD.Load("res://Lauri/RadarBlip.tscn");

	public override void _Ready()
	{
		_NodeIdToBlip = new Dictionary<ulong, RadarBlip>();
	}

	public override void _Process(double delta)
	{
		HandleRadarEntityLocations();
	}

	private void HandleRadarEntityLocations()
	{
		foreach (var nodeIdToNode in NodeCollection.Instance.NodesToTrack)
		{
			RadarBlip existingBlip = null;
			ulong nodeId = nodeIdToNode.Key;
			Node2D node = nodeIdToNode.Value;
			if (!_NodeIdToBlip.TryGetValue(nodeId, out existingBlip))
			{
				RadarBlip newBlip = (RadarBlip)blipScene.Instantiate();
				newBlip.Position = node.GlobalPosition / 10;
				newBlip.Rotation = node.GlobalRotation / 10;
				//GD.Print(Position);
				//GD.Print(Rotation);
				this.AddChild(newBlip);
				_NodeIdToBlip[nodeId] = newBlip;
			}
			else
			{
				existingBlip.Position = node.GlobalPosition / 10;
				existingBlip.Rotation = node.GlobalRotation;
			}
			//GD.Print("Entity " + nodeId + " global position: " + node.GlobalPosition.ToString());
		}

		var nodesIdsToUnregister = new Array<ulong>();
		foreach (var nodeIdToNode in NodeCollection.Instance.NodesToUnRegister)
		{
			ulong nodeId = nodeIdToNode.Key;
			RadarBlip blipToFree = null;
			if (_NodeIdToBlip.TryGetValue(nodeId, out blipToFree)) 
			{
				blipToFree.QueueFree();
			}
			//GD.Print("Remove Entity " + nodeId);
			_NodeIdToBlip.Remove(nodeId);
			nodesIdsToUnregister.Add(nodeId);
		}

		foreach (ulong unregisteredNodeId in nodesIdsToUnregister)
		{
			NodeCollection.Instance.NodesToUnRegister.Remove(unregisteredNodeId);
		}
	}
}
