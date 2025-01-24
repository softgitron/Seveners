using Godot;
using NoiseTest;

public partial class Terrain : TileMapLayer
{
	[Export]
	int MAP_SIZE_X = 512;
	[Export]
	int MAP_SIZE_Y = 512;
	const double GLANURARITY = 0.03;
	const double SEABED_UPPER_BOUNDARY = -0.8;
	const double ROCK_UPPER_BOUNDARY = 0.1;
	const double SEA_UPPER_BOUNDARY = 0.7;
	public readonly Vector2I SEABED = new Vector2I(0, 0);
	public readonly Vector2I ROCK = new Vector2I(1, 0);
	public readonly Vector2I SEA = new Vector2I(2, 0);
	public readonly Vector2I ISLAND = new Vector2I(3, 0);

	public override void _Ready()
	{
		var noise = new OpenSimplexNoise();
		for (var y = -MAP_SIZE_Y / 2; y < MAP_SIZE_Y / 2; y++)
		{
			for (var x = -MAP_SIZE_X / 2; x < MAP_SIZE_X / 2; x++)
			{
				Vector2I texture;
				var temporaryVector = new Vector2I(x, y);

				var noiseValue = noise.Evaluate(x * GLANURARITY, y * GLANURARITY);

				// Skip if tile is already defined.
				var currentAtlasCoords = GetCellAtlasCoords(temporaryVector);
				if (currentAtlasCoords != new Vector2(-1, -1))
				{
					continue;
				}

				if (noiseValue < SEABED_UPPER_BOUNDARY)
					texture = SEABED;
				else if (noiseValue < ROCK_UPPER_BOUNDARY)
					texture = ROCK;
				else if (noiseValue < SEA_UPPER_BOUNDARY)
					texture = SEA;
				else
					texture = ISLAND;
				SetCell(new Vector2I(x, y), 0, texture);
			}
		}
	}
}
