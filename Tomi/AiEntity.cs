using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AiEntity : CharacterBody2D
{
	[Export]
	public float _movementSpeed = 1000f;
	[Export]
	public float _turnSpeed = 0.01f;
	[Export]
	public Marker2D _movementTarget;
	[Export]
	public Path2D _path;

	private const float CorrectionAngle = (float)Math.PI / 2;

	private Terrain terrain;
	private List<Vector2> navigationPoints = [];
	private Vector2 currentNavigationTarget;

	public override void _Ready()
	{
		CallDeferred("SetMovementTarget");
		terrain = GetNode<Terrain>("../MapGeneration/Above Water");
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

	public override void _PhysicsProcess(double delta)
	{
		if (navigationPoints.Count == 0)
		{
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
		MoveAndSlide();
	}
}
