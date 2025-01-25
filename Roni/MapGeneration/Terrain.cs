using Godot;

public partial class Terrain : TileMapLayer
{
	[Export]
	Texture2D Noise;
	const double SEABED_UPPER_BOUNDARY = 0.4;
	const double SEA_UPPER_BOUNDARY = 0.8;
	const double ROCK_UPPER_BOUNDARY = 0.85;
	const double BEACH_UPPER_BOUNDARY = 0.87;
	public readonly Vector2I SEABED = new(8, 0);
	public readonly Vector2I ROCK = new(5, 0);
	public readonly Vector2I SEA = new(6, 0);
	public readonly Vector2I BEACH = new(2, 0);
	public readonly Vector2I ISLAND_CENTER = new(0, 0);

	private bool _generated = false;

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (!_generated)
		{
			_generated = Generate();
		}
	}

	private bool Generate()
	{
		var textureImage = Noise.GetImage();
		if (textureImage == null)
		{
			return false;
		}

		var halfHeight = Noise.GetHeight() / 2;
		var halfWidth = Noise.GetWidth() / 2;

		for (var y = -halfHeight; y < halfHeight; y++)
		{
			for (var x = -halfWidth; x < halfWidth; x++)
			{
				Vector2I texture;
				var temporaryVector = new Vector2I(x, y);

				var noiseValue = textureImage.GetPixel(x + halfWidth, y + halfHeight).R;

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
