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
	public float _movementSpeed = 500f;
	[Export]
	public float _turnSpeed = 0.01f;
	[Export]
	public Marker2D _movementTarget;
	[Export]
	public Path2D _path;
	[Export]
	public Node2D _torpedoLaunch;
	[Export]
	public Terrain terrain;
	[Export]
	public Timer FireTimer;

	private PackedScene bulletScene = (PackedScene)GD.Load("res://Juuso/TorpedoEnemy.tscn");
	private const float CorrectionAngle = (float)Math.PI / 2;
	private bool HasReachedDestination = false;
	private List<Vector2> navigationPoints = [];
	private Vector2 currentNavigationTarget;
	private bool firing = false;
	private Node2D player = null;

	private float health = 100;
	private bool isDead;

	public void TakeDamage(float damage)
	{
		health -= damage;
		if (health <= 0 && !isDead)
		{
			isDead = true;
			PlayerStats stats = GetTree().Root.GetNode<PlayerStats>("Main5/CanvasLayer/PlayerStats");
			stats.enemiesDestroyed++;
			stats.enemiesText.Text = "Enemies Destroyed: "+stats.enemiesDestroyed;
		}
	}

	public override void _Ready()
	{
		var random = new Random();
		FireTimer.WaitTime = 1.8 + random.NextDouble() / 4;
		terrain = GetNode<Terrain>("../../../Above Water");
		CallDeferred("SetMovementTarget");
		NodeCollection.Instance.RegisterNode(this);
	}

	public void SetMovementTarget()
	{
		//Debug.Print("Targetting terrain coordinate...: " + _movementTarget.ToString());
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
			}

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

	public override void _Process(double delta)
	{
		if (health <= 0)
		{
			Modulate -= new Color(0, 0, 0, (float)delta);
			if (Modulate.A <= 0)
			{
				QueueFree();
			}
		}
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

	public void _on_player_detection_area_body_entered(Node2D node)
	{
		if (node is not HumanControllableSubmarine)
		{
			return;
		}
		player = node;
	}

	public void _on_player_detection_area_body_exited(Node2D node)
	{
		if (node is not HumanControllableSubmarine)
		{
			return;
		}

		player = null;
		AssignNewTarget();
		SetMovementTarget();
	}

	public void _on_fire_area_body_entered(Node2D node)
	{
		if (node is HumanControllableSubmarine)
		{
			firing = true;
		}
	}

	public void _on_fire_area_body_exited(Node2D node)
	{
		if (node is HumanControllableSubmarine)
		{
			firing = false;
		}
	}

	public void _on_timer_timeout()
	{
		if (health > 0)
		{
			Fire();
		}
	}

	public void _on_navigation_timer_timeout()
	{
		if (player == null)
		{
			return;
		}

		_movementTarget.GlobalPosition = player.GlobalPosition;
		SetMovementTarget();
	}

	public void Fire()
	{
		if (!firing)
		{
			return;
		}

		Torpedo torpedo = (Torpedo)bulletScene.Instantiate();
		torpedo.pos = _torpedoLaunch.GlobalPosition;
		torpedo.direction = _torpedoLaunch.GlobalRotation;
		torpedo.rotation = _torpedoLaunch.GlobalRotation;
		GetTree().Root.AddChild(torpedo);
	}
}
