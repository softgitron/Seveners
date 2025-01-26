using System.Diagnostics;
using Godot;

public partial class Terrain : TileMapLayer
{
	public const int Width = 512;
	public const int Height = 512;
	[Export] public bool UnderTheWater = false;
	[Export] public int SafetyLimit = 60;
	[Export] public int ShallowLimit = 5;
	[Export] public int BeachLimit = -10;
	[Export] public int BoundaryDepth = 10;

	// Values go from -0.5 to 0.5
	const double NOISE_VALUE_NORMAL_W = -0.4;
	const double NOISE_VALUE_SHALLOW_W = 0.25;
	const double NOISE_VALUE_SAND = 0.35;
	const double NOISE_VALUE_GRASS = 0.40;
	const double NOISE_VALUE_HILL = 0.45;
	const double NOISE_VALUE_MOUNTAIN = 0.48;

	public readonly Vector2I Sand0 = new(0, 0);
	public readonly Vector2I Sand1 = new(0, 1);
	public readonly Vector2I Grass0 = new(1, 0);
	public readonly Vector2I Grass1 = new(1, 1);
	public readonly Vector2I Hill0 = new(2, 0);
	public readonly Vector2I Hill1 = new(2, 1);
	public readonly Vector2I Mountain0 = new(3, 0);
	public readonly Vector2I Mountain1 = new(3, 1);
	public readonly Vector2I ShallowW0 = new(7, 0);
	public readonly Vector2I ShallowW1 = new(7, 1);
	public readonly Vector2I NormalW0 = new(8, 0);
	public readonly Vector2I NormalW1 = new(8, 1);
	public readonly Vector2I DeepW0 = new(9, 0);
	public readonly Vector2I DeepW1 = new(9, 1);

	public AStarGrid2D NavigationAgent;

	private FastNoiseLite Noise;

	public void Initialize(int seed)
	{
		Noise = new()
		{
			Frequency = 0.01f,
			FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
			FractalOctaves = 5,
			FractalLacunarity = 1.6f,
			Seed = seed
		};
		NavigationAgent = new()
		{
			Region = new Rect2I(0, 0, Width+2*BoundaryDepth, Height+ 2 * BoundaryDepth),
			DiagonalMode = AStarGrid2D.DiagonalModeEnum.AtLeastOneWalkable,
			DefaultComputeHeuristic = AStarGrid2D.Heuristic.Octile,
			DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Octile
		};
		NavigationAgent.Update();

		Generate();
	}

	public bool validateMapBounds(Vector2 vector)
	{
		if (vector.X < SafetyLimit || vector.X > (Width * 6 - SafetyLimit))
		{
			return false;
		}
		if (vector.Y < SafetyLimit || vector.Y > (Height * 6 - SafetyLimit))
		{
			return false;
		}

		return true;
	}

	public bool CanSpawnOn(Vector2 position)
	{
		var terrainCoordinates = WorldCoordinateToTerrainCoordinate(position);
		var isPointSolid = NavigationAgent.IsPointSolid(terrainCoordinates);

		if (isPointSolid)
			return false;
		return true;
	}

	public Vector2I WorldCoordinateToTerrainCoordinate(Vector2 globalCoordinate)
	{
		var localPosition = this.ToLocal(globalCoordinate);
		var terrainCoordinate = this.LocalToMap(localPosition);
		return terrainCoordinate;
	}

	public Vector2 TerrainCoordinateToWorldCoordinate(Vector2 terrainCoordinate)
	{
		var terrainInterface = (Vector2I)terrainCoordinate;
		var localPosition = this.MapToLocal(terrainInterface);
		var globalPosition = this.ToGlobal(localPosition);
		return globalPosition;
	}

	private bool Generate()
	{
		GenerateMapBoundary();

		for (var y = BoundaryDepth; y < Height+BoundaryDepth; y++)
		{
			for (var x = BoundaryDepth; x < Width+BoundaryDepth; x++)
			{
				var coordinate = new Vector2I(x, y);
				Vector2I texture;
				var noiseValue = Noise.GetNoise2D(x, y);

				if (noiseValue > NOISE_VALUE_MOUNTAIN)
				{
					if (UnderTheWater)
					{
						texture = Mountain1;
					}
					else
					{
						texture = Mountain0;
					}
					NavigationAgent.SetPointSolid(coordinate);
				}
				else if (noiseValue > NOISE_VALUE_HILL)
				{
					if (UnderTheWater)
					{
						texture = Hill1;
					}
					else
					{
						texture = Hill0;
					}
					NavigationAgent.SetPointSolid(coordinate);
				}
				else if (noiseValue > NOISE_VALUE_GRASS)
				{
					if (UnderTheWater)
					{
						texture = Grass1;
					}
					else
					{
						texture = Grass0;
					}
					NavigationAgent.SetPointSolid(coordinate);
				}
				else if (noiseValue > NOISE_VALUE_SAND)
				{
					if (UnderTheWater)
					{
						texture = Sand1;
					}
					else
					{
						texture = Sand0;
					}
					NavigationAgent.SetPointSolid(coordinate);
				}
				else if (noiseValue > NOISE_VALUE_SHALLOW_W)
				{
					if (UnderTheWater)
					{
						texture = ShallowW1;
					}
					else
					{
						texture = ShallowW0;
					}
					NavigationAgent.SetPointSolid(coordinate);
				}
				else if (noiseValue > NOISE_VALUE_NORMAL_W)
				{
					if (UnderTheWater)
					{
						texture = NormalW1;
					}
					else
					{
						texture = NormalW0;
					}
				}
				else
				{
					if (UnderTheWater)
					{
						texture = DeepW1;
					}
					else
					{
						texture = DeepW0;
					}
				}
				SetCell(new Vector2I(x, y), 0, texture);
			}
		}

		return true;
	}

	public void GenerateMapBoundary() {

        Debug.Print("Starting to create Boundary");
        for (var y = 0; y < (BoundaryDepth*2 + Height); y++)
		{


			for (var x = 0; x < (BoundaryDepth*2+Width); x++)
			{
                if (x >= BoundaryDepth && x < Width + BoundaryDepth && y >= BoundaryDepth && y < Height + BoundaryDepth)
                {
                    Debug.Print("In middle X: " + x);
                    continue;
                }

                Debug.Print("Trying to create BoundaryCell");
                var coordinate = new Vector2I(x, y);
                Vector2I texture;
                if (UnderTheWater)
                {
                    texture = Sand1;
                }
                else
                {
                    texture = Sand0;
                }
                NavigationAgent.SetPointSolid(coordinate);
                SetCell(coordinate, 0, texture);
            }
		}
        Debug.Print("Finished Creating Boundary");
    }


	//public bool GenerateMapBoundary()
	//{
	//	for (var y = 0; y < (BoundaryDepth*2 + Height); y++)
	//	{
	//		if (y >= BoundaryDepth || y < Height+BoundaryDepth)
	//		{
	//			continue;
	//		}
	//		for (var x = 0; x < (BoundaryDepth*2 + Width); x++)
	//		{
	//			if (x >= BoundaryDepth || x < Width+BoundaryDepth)
	//			{
	//				continue;
	//			}
	//			Debug.Print("Trying to create BoundaryCell");
 //               var coordinate = new Vector2I(x, y);
 //               Vector2I texture;
 //               if (UnderTheWater)
 //               {
 //                   texture = Sand1;
 //               }
 //               else
 //               {
 //                   texture = Sand0;
 //               }
 //               NavigationAgent.SetPointSolid(coordinate);
 //               SetCell(new Vector2I(x, y), 0, texture);
 //           }
	//	}
	//	return true;
	//}
}
