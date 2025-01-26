using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class Radar : Sprite2D
{
	private Dictionary<ulong, RadarBlip> _NodeIdToBlip;
	PackedScene blipScene = (PackedScene)GD.Load("res://Lauri/RadarBlip.tscn");
	private HumanControllableSubmarine _playerNode = null;
	private Terrain _terrain = null;
	private Camera2D _radarCameraNode = null;
	[Export]
	private NodePath _playerNodePath = new NodePath("");
	[Export]
	private NodePath _radarCameraPath = null;
	private int RadarScaleToWorldInPixels = NodeCollection.RadarScale;

	private const int RadarPixelHeight = 400;
	private const int RadarPixelWidth = 400;

	public override void _Ready()
	{
		// jsut hardcode because hurry
		const int PixelMultiplier = 6;
		RadarScaleToWorldInPixels = (Terrain.Height * PixelMultiplier) / RadarPixelHeight;

		_NodeIdToBlip = new Dictionary<ulong, RadarBlip>();
		_playerNode = GetTree().Root.GetNode<HumanControllableSubmarine>("Main Root/Player");
		Debug.Assert(_playerNode != null, "Player node can not be null! Radar can not work!");
		_radarCameraNode = GetNodeOrNull<Camera2D>(_radarCameraPath);
		Debug.Assert(_radarCameraNode != null, "Radar camera node can not be null! Radar can not work!");
		_terrain = GetTree().Root.GetNode<Terrain>("Main Root/Above Water");
		Debug.Assert(_terrain != null, "Terrain node can not be null! Radar can not work!");
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
		Vector2 radarOffsetScaledToRadar = radarOffset / RadarScaleToWorldInPixels;
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
				newBlip.Position = node.GlobalPosition / RadarScaleToWorldInPixels;
				newBlip.Rotation = node.GlobalRotation / RadarScaleToWorldInPixels;
				if (node is Goal)
				{
					newBlip.Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
				}
				//GD.Print(Position);
				//GD.Print(Rotation);
				this.AddChild(newBlip);
				_NodeIdToBlip[nodeId] = newBlip;
			}
			else
			{
				existingBlip.Position = node.GlobalPosition / RadarScaleToWorldInPixels;
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
