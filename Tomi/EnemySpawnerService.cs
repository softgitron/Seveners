using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class EnemySpawnerService : Node2D
{
	//[Export]
	PackedScene _enemy;
	[Export]
	int enemyCount;

	private const int PIXEL_SIZE = 6;

	private Terrain _terrain = null;

	//private static EnemySpawnerService _spawnerServiceInstance = null;

	//public static EnemySpawnerService GetSpawnerService(Terrain terrain)
	//{
	//	if (_spawnerServiceInstance == null)
	//	{
	//		_spawnerServiceInstance = new EnemySpawnerService(terrain);
	//	}

	//	return _spawnerServiceInstance;
	//}

	//private EnemySpawnerService(Terrain terrain)
	//{
	//	_terrain = terrain;
	//}

	public void Initialize(Terrain terrain)
	{
		_terrain = terrain;
	}

	public void CreateEnemies()
	{
		var random = new Random();
		var maxTries = 100;
		var tries = 0;

		for (int i = 0; i < enemyCount; i++)
		{
			var isPlacementValid = false;

			while (!isPlacementValid)
			{
				if (tries >= maxTries)
				{
					break;
				}
				tries++;
				var newRandomLocationSuggestion = getRandomLocation(random);

				isPlacementValid = TryPlaceShip(newRandomLocationSuggestion);
				if (isPlacementValid)
				{
					var newEnemy = (Node2D)_enemy.Instantiate();
					newEnemy.GlobalPosition = newRandomLocationSuggestion;
					AddChild(newEnemy);
					Debug.Print("Enemy created at: " + newRandomLocationSuggestion.ToString());
				}
			}
		}
	}

	private Vector2 getRandomLocation(Random random)
	{

		var randomY = random.Next(_terrain.SafetyLimit, Terrain.Height * PIXEL_SIZE - _terrain.SafetyLimit);
		var randomX = random.Next(_terrain.SafetyLimit, Terrain.Width * PIXEL_SIZE - _terrain.SafetyLimit);

		return new Vector2(randomX, randomY);
	}

	private bool TryPlaceShip(Vector2 spawnPoint)
	{
		var collisionBoxCoords = GetCollisionBoxCoords(spawnPoint);

		foreach (Vector2 spawnPos in collisionBoxCoords)
		{
			if (!_terrain.CanSpawnOn(spawnPos))
			{
				Debug.Print(spawnPos.ToString());
				return false;
			}
		}
		return true;
	}

	private List<Vector2> GetCollisionBoxCoords(Vector2 spawnPoint)
	{
		var collisionBoxCorners = new List<Vector2>
		{
			GetDynamicCoordFrom(10, 25, spawnPoint),
			GetDynamicCoordFrom(-10, -25, spawnPoint),
			GetDynamicCoordFrom(10, -25, spawnPoint),
			GetDynamicCoordFrom(-10, 25, spawnPoint)
		};

		return collisionBoxCorners;
	}

	private Vector2 GetDynamicCoordFrom(int width, int height, Vector2 pointFrom)
	{
		var newXCoord = pointFrom.X + width;
		var newYCoord = pointFrom.Y + height;

		return new Vector2(newXCoord, newYCoord);
	}

	//Called when the node enters the scene tree for the first time.
	public void _on_timer_timeout()
	{
		Debug.Print("Starting enemy gen...");
		_enemy = (PackedScene)GD.Load("res://Roni/Enemy.tscn");
		_terrain = GetNode<Terrain>("../Above Water");
		CreateEnemies();
		Debug.Print("Done creating enemies");
	}
}
