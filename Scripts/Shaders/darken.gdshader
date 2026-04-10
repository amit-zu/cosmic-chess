shader_type canvas_item;

uniform float darkness : hint_range(0.0, 1.0) = 1.0;

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    // Interpolate between original color and black
    COLOR = mix(tex_color, vec4(0.0, 0.0, 0.0, tex_color.a), darkness);
}
