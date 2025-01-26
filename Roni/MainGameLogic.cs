using System;
using Godot;

public partial class MainGameLogic : Node
{
	[Export]
	public Terrain Terrain;

	[Export]
	public HumanControllableSubmarine Player;

	[Export]
	public Area2D Goal;

	[Export]
	public Terrain AboveWater;

	[Export]
	public Terrain BelowWater;

    [Export]
    public EnemySpawnerService SpawnerService;

    public int level = 1;

	private Random Random = new();

	public override void _Ready()
	{
		base._Ready();
		Initialize();
	}

	private void Initialize()
	{
		Player.Reset();
		GenerateMap();
		//SpawnerService.Initialize(AboveWater);
		//SpawnerService.CreateEnemies();
		SpawnPlayerAndGoal();
	}

	private void GenerateMap()
	{
		var seed = Random.Next();
		AboveWater.Initialize(seed);
		BelowWater.Initialize(seed);
	}


	private void SpawnPlayerAndGoal()
	{
		while (true)
		{
			var globalWidth = Terrain.Width * Terrain.TileSet.TileSize.X;
			var globalHeight = Terrain.Height * Terrain.TileSet.TileSize.Y;
			var margin = Convert.ToInt32(globalWidth * 0.2);

			var offset = new Vector2(globalWidth / 2, globalHeight / 2);
			var minimumDistance = globalWidth / 1.5;
			var waypointDistance = globalWidth / 2 - margin;

			var playerPosition = GetRandomWaypointForDistance(waypointDistance, margin) + offset;
			var playerGoal = GetRandomWaypointForDistance(waypointDistance, margin) + offset;

			if (playerPosition.DistanceTo(playerGoal) < minimumDistance)
			{
				continue;
			}

			var playerMapPosition = WorldCoordinateToTerrainCoordinate(playerPosition);
			var playerGoalMapPosition = WorldCoordinateToTerrainCoordinate(playerGoal);

			if (IsSpawnObscured(playerMapPosition))
			{
				continue;
			}

			if (IsSpawnObscured(playerGoalMapPosition))
			{
				continue;
			}

			try
			{
				Terrain.NavigationAgent.GetPointPath(playerMapPosition, playerGoalMapPosition);
			}
			catch (Exception)
			{
				continue;
			}

			Player.GlobalPosition = playerPosition;
			Goal.GlobalPosition = playerGoal;

			break;
		}
	}

	public void _on_goal_body_entered(Node2D node)
	{
		if (node is HumanControllableSubmarine)
		{
			level++;
			Initialize();
		}
	}

	private bool IsSpawnObscured(Vector2I mapCoordinate)
	{
		var area = 15;
		for (var x = mapCoordinate.X - area; x <= mapCoordinate.X + area; x++)
		{
			for (var y = mapCoordinate.Y - area; y <= mapCoordinate.Y + area; y++)
			{
				if (Terrain.NavigationAgent.IsInBounds(x, y) && Terrain.NavigationAgent.IsPointSolid(new Vector2I(x, y)))
				{
					return true;
				}
			}
		}
		return false;
	}

	private Vector2 GetRandomWaypointForDistance(float distanceToWaypoint, int distanceFlux)
	{
		var randomCirclePoint = Random.Next(0, 360);
		var randomFlux = Random.Next(-distanceFlux, distanceFlux);

		var randomXValue = distanceToWaypoint * Math.Cos(randomCirclePoint) + randomFlux;
		var randomYValue = distanceToWaypoint * Math.Sin(randomCirclePoint) + randomFlux;

		return new Vector2((float)randomXValue, (float)randomYValue);
	}

	private Vector2I WorldCoordinateToTerrainCoordinate(Vector2 globalCoordinate)
	{
		var localPosition = Terrain.ToLocal(globalCoordinate);
		var terrainCoordinate = Terrain.LocalToMap(localPosition);
		return terrainCoordinate;
	}
}
