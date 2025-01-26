using Godot;
using System;

public partial class RadarViewPortContainer : SubViewportContainer
{
	private const int RadarPixelHeight = 400;
	private const int RadarPixelWidth = 400;
	
	public override void _Ready()
	{
		const int PixelMultiplier = 6;
		const int radarScale = (Terrain.Height * PixelMultiplier) / RadarPixelHeight;

		//int radarHeight = (Terrain.Height * PixelMultiplier) / radarScale;
		//int radarWidth = (Terrain.Width * PixelMultiplier) / radarScale;
		int radarHeight = RadarPixelHeight;
		int radarWidth = RadarPixelWidth;
		Vector2I viewPortSize = new Vector2I(radarHeight, radarWidth);
		GD.Print("ViewPort" + viewPortSize.ToString());
		this.Size = viewPortSize;

	}
}
