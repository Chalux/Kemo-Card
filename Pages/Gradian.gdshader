shader_type canvas_item;

uniform vec4 color1:source_color;
uniform vec4 color2:source_color;

void fragment() {
	COLOR = mix(color1, color2, UV.x);
	// Called for every pixel the material is visible on.
}
