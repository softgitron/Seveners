using Godot;

public partial class Terrain : TileMapLayer
{
	[Export]
	public int Width = 512;
	public int Height = 512;
	private FastNoiseLite Noise;

	// Values go from -0.5 to 0.5
	const double NOISE_VALUE_NORMAL_W = -0.4;
	const double NOISE_VALUE_SHALLOW_W = 0.1;
	const double NOISE_VALUE_SAND = 0.2;
	const double NOISE_VALUE_GRASS = 0.25;
	const double NOISE_VALUE_HILL = 0.45;
	const double NOISE_VALUE_MOUNTAIN = 0.48;

	public readonly Vector2I Sand0 = new (0,0);
	public readonly Vector2I Sand1 = new (0,1);
	public readonly Vector2I Grass0 = new (1,0);
	public readonly Vector2I Hill0 = new (2,0);
	public readonly Vector2I Mountain0 = new (3,0);
	public readonly Vector2I ShallowW0 = new (7,0);
	public readonly Vector2I NormalW0 = new (8,0);
	public readonly Vector2I DeepW0 = new (9,0);

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
				if (noiseValue > NOISE_VALUE_MOUNTAIN){
					texture = Mountain0;
				} else if (noiseValue > NOISE_VALUE_HILL){
					texture = Hill0;
				} else if (noiseValue > NOISE_VALUE_GRASS){
					texture = Grass0;
				} else if (noiseValue > NOISE_VALUE_SAND){
					texture = Sand0;
				} else if (noiseValue > NOISE_VALUE_SHALLOW_W){
					texture = ShallowW0;
				} else if (noiseValue > NOISE_VALUE_NORMAL_W){
					texture = NormalW0;
				} else {
					texture = DeepW0;
				}
				SetCell(new Vector2I(x, y), 0, texture);
			}
		}

		return true;
	}
}
