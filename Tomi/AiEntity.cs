using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

public partial class AiEntity : CharacterBody2D
{
	
	[Export]
	public float _randomWaypointDistanceMultiplier = 1;
	[Export]
	public float _movementSpeed = 1000f;
	[Export]
	public float _turnSpeed = 0.01f;
	[Export]
	public Marker2D _movementTarget;
	[Export]
	public Path2D _path;

	private const float CorrectionAngle = (float)Math.PI / 2;
	private bool HasReachedDestination = false;

	private Terrain terrain;
	private List<Vector2> navigationPoints = [];
	private Vector2 currentNavigationTarget;

	public override void _Ready()
	{
		CallDeferred("SetMovementTarget");
		terrain = GetNode<Terrain>("../../Above Water");
		NodeCollection.Instance.RegisterNode(this);
	}

	public void SetMovementTarget()
	{
		var targetTerrainCoordinate = WorldCoordinateToTerrainCoordinate(_movementTarget.GlobalPosition);
		var currentTerrainCoordinate = WorldCoordinateToTerrainCoordinate(GlobalPosition);

		var mapNavigationPoints = terrain.NavigationAgent.GetPointPath(currentTerrainCoordinate, targetTerrainCoordinate, true);
		navigationPoints = [.. mapNavigationPoints.Select(TerrainCoordinateToWorldCoordinate)];
		currentNavigationTarget = navigationPoints[0];
		navigationPoints.RemoveAt(0);
	}

	private Vector2 GetRandomWaypointForDistance(float distanceToWaypoint)
	{
		var random = new Random();
		var randomCirclePoint = random.Next(0, 360);

		var randomXValue = distanceToWaypoint * Math.Cos(randomCirclePoint);
		var randomYValue = distanceToWaypoint * Math.Sin(randomCirclePoint);

		return new Vector2((float)randomXValue, (float)randomYValue);
	}

	private Vector2I WorldCoordinateToTerrainCoordinate(Vector2 globalCoordinate)
	{
		var localPosition = terrain.ToLocal(globalCoordinate);
		var terrainCoordinate = terrain.LocalToMap(localPosition);
		return terrainCoordinate;
	}

	private Vector2 TerrainCoordinateToWorldCoordinate(Vector2 terrainCoordinate)
	{
		var terrainInterface = (Vector2I)terrainCoordinate;
		var localPosition = terrain.MapToLocal(terrainInterface);
		var globaPosition = terrain.ToGlobal(localPosition);
		return globaPosition;
	}

	private void AssignNewTarget()
	{
		var isTargetValid = false;
		var random = new Random();
		var newTargetForPatrol = GlobalPosition;



		while (!isTargetValid)
		{
			var wayPointDistance = random.Next(1, (int)Math.Round(500 * _randomWaypointDistanceMultiplier, 0));

			newTargetForPatrol = GetRandomWaypointForDistance(wayPointDistance);

			if (!terrain.validateMapBounds(newTargetForPatrol))
			{
				continue;
			};

			var mapNavigationPoints = terrain.NavigationAgent.GetPointPath(WorldCoordinateToTerrainCoordinate(GlobalPosition), WorldCoordinateToTerrainCoordinate(newTargetForPatrol), false);

			if (mapNavigationPoints.Length != 0)
			{
				isTargetValid = true;
			}
		}
		_movementTarget.GlobalPosition = newTargetForPatrol;
	}
	
	public override void _Notification(int what)
	{
		if (what == NotificationExitTree) NodeCollection.Instance.UnregisterNode(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (navigationPoints.Count == 0)
		{
			AssignNewTarget();
			SetMovementTarget();
			return;
		}

		if (currentNavigationTarget.DistanceTo(GlobalPosition) < 5.0)
		{
			currentNavigationTarget = navigationPoints.First();
			navigationPoints.RemoveAt(0);
		}

		Vector2 newVelocity = currentNavigationTarget - GlobalPosition;
		newVelocity = newVelocity.Normalized();
		newVelocity *= _movementSpeed * (float)delta;

		var targetVector = currentNavigationTarget - GlobalPosition;

		GlobalRotation = Mathf.LerpAngle(GlobalRotation, targetVector.Angle() + CorrectionAngle, _turnSpeed);
		Velocity = newVelocity;

		// Ei toimi jostain syystä saatana. Ei mee signaalit perille.
		//SignalBus.Instance.EmitSignal(SignalBus.RadarLocationRegisteredName, GlobalPosition);
		MoveAndSlide();
	}
}
