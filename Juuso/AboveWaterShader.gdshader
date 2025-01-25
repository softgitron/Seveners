shader_type canvas_item;

uniform bool isUnderWater;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 pixelColor = texture(TEXTURE, UV);
	if (isUnderWater){
		COLOR = vec4(0,0,0,0);
	} else {
		COLOR = pixelColor;
	}
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
