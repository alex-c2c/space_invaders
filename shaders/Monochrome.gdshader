shader_type canvas_item;

void fragment() {
	COLOR = texture(TEXTURE, UV);
  	float avg = (COLOR.r + COLOR.g + COLOR.b) / 3.0f;
    COLOR.rgb = vec3(avg);
	COLOR.a = 0.5f;
}
