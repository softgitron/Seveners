using Godot;
using NoiseTest;
using System;

public partial class JuusonNoise : Sprite2D
{

	private NoiseTexture2D noise;
	public override void _Ready(){
		noise = Texture as NoiseTexture2D;
	}
	public override void _Process(double delta){
		FastNoiseLite fnl = noise.Noise as FastNoiseLite;
		fnl.Offset = new Vector3(fnl.Offset.X+ 0.5f, fnl.Offset.Y+ 0.5f, 0);
	}
}
