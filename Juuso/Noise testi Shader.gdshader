shader_type canvas_item;

uniform bool isUnderWater;
uniform vec4 waterColor : source_color;
uniform vec4 landColor : source_color;
uniform vec4 sandColor : source_color;
uniform vec4 deepWaterColor : source_color;

uniform vec4 bottomColor : source_color;
uniform vec4 oceanColor : source_color;
uniform vec4 wallsColor : source_color;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 pixelColor = texture(TEXTURE, UV);
	if (isUnderWater){
		if (pixelColor.r < 0.4){
			COLOR = bottomColor;
		} else if (pixelColor.r < 0.5){
			COLOR = oceanColor;
		} else {
			COLOR = wallsColor;
		}
	} else {
		if (pixelColor.r < 0.4){
			COLOR = deepWaterColor;
		} else if (pixelColor.r < 0.7){
			COLOR = waterColor;
		} else if (pixelColor.r < 0.8){
			COLOR = sandColor;
		} else {
			COLOR = landColor;
		}
	}
	
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
