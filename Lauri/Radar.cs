using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class Radar : Sprite2D
{
	private Dictionary<ulong, RadarBlip> _NodeIdToBlip;
	PackedScene blipScene = (PackedScene)GD.Load("res://Lauri/RadarBlip.tscn");
	private HumanControllableSubmarine _playerNode = null;
	private Camera2D _radarCameraNode = null;
	[Export]
	private NodePath _playerNodePath = new NodePath("");
	[Export]
	private NodePath _radarCameraPath = null;

	public override void _Ready()
	{
		_NodeIdToBlip = new Dictionary<ulong, RadarBlip>();
		_playerNode = GetTree().Root.GetNode<HumanControllableSubmarine>("Main2/Player");
		Debug.Assert(_playerNode != null, "Player node can not be null! Radar can not work!");
		_radarCameraNode = GetNodeOrNull<Camera2D>(_radarCameraPath);
		Debug.Assert(_radarCameraNode != null, "Radar camera node can not be null! Radar can not work!");
	}

	public override void _Process(double delta)
	{
		SnapCameraToPlayer();
		HandleRadarEntityLocations();
	}
	
	private void SnapCameraToPlayer()
	{
		//Debug.Print("Camera global position: " + _radarCameraNode.GlobalPosition.ToString());
		//Debug.Print("Player global position: " + _playerNode.GlobalPosition.ToString());
		//_radarCameraNode.Position = _playerNode.GlobalPosition;
		//this.GlobalPosition = _playerNode.GlobalPosition;
		//_radarCameraNode.Offset = _playerNode.Position;
		Vector2 radarOffset = _playerNode.GlobalPosition;
		Vector2 radarOffsetScaledToRadar = radarOffset / 10;
		_radarCameraNode.Offset = radarOffsetScaledToRadar;
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
