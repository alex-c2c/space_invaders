shader_type canvas_item;
uniform bool _isOn = false;

void fragment() {
	if (_isOn)
	{
		COLOR = texture(TEXTURE, UV);
  		float avg = (COLOR.r + COLOR.g + COLOR.b) / 3.0f;
    	COLOR = vec4(avg, avg, avg, 0.5f);
	}
}
