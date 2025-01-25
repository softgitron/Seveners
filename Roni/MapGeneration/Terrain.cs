using Godot;

public partial class Terrain : TileMapLayer
{
	[Export]
	public int Width = 512;
	public int Height = 512;
	private FastNoiseLite Noise;
	const double SEABED_UPPER_BOUNDARY = -0.1;
	const double SEA_UPPER_BOUNDARY = 0.3;
	const double ROCK_UPPER_BOUNDARY = 0.35;
	const double BEACH_UPPER_BOUNDARY = 0.37;
	public readonly Vector2I SEABED = new(8, 0);
	public readonly Vector2I ROCK = new(5, 0);
	public readonly Vector2I SEA = new(6, 0);
	public readonly Vector2I BEACH = new(2, 0);
	public readonly Vector2I ISLAND_CENTER = new(0, 0);

	private bool _generated = false;

	public override void _Ready()
	{
		base._Ready();
		Noise = new()
		{
			Frequency = 0.01f,
			FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
			FractalOctaves = 5,
			FractalLacunarity = 1.6f,
		};
		Generate();
	}

	private bool Generate()
	{
		var halfHeight = Height / 2;
		var halfWidth = Width / 2;

		for (var y = -halfHeight; y < halfHeight; y++)
		{
			for (var x = -halfWidth; x < halfWidth; x++)
			{
				Vector2I texture;
				var temporaryVector = new Vector2I(x, y);

				var noiseValue = Noise.GetNoise2D(x, y);

				// Skip if tile is already defined.
				var currentAtlasCoords = GetCellAtlasCoords(temporaryVector);
				if (currentAtlasCoords != new Vector2(-1, -1))
				{
					continue;
				}

				if (noiseValue < SEABED_UPPER_BOUNDARY)
					texture = SEABED;
				else if (noiseValue < SEA_UPPER_BOUNDARY)
					texture = SEA;
				else if (noiseValue < ROCK_UPPER_BOUNDARY)
					texture = ROCK;
				else if (noiseValue < BEACH_UPPER_BOUNDARY)
					texture = BEACH;
				else
					texture = ISLAND_CENTER;
				SetCell(new Vector2I(x, y), 0, texture);
			}
		}

		return true;
	}
}
