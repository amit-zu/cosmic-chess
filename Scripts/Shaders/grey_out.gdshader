shader_type canvas_item;

uniform float desat : hint_range(0.0, 1.0) = 1.0; // 1 = full greyscale
uniform float dim   : hint_range(0.0, 1.0) = 0.6; // 1 = no dim

void fragment() {
    vec4 c = texture(TEXTURE, UV);
    float l = dot(c.rgb, vec3(0.299, 0.587, 0.114));
    c.rgb = mix(c.rgb, vec3(l), desat);
    c.rgb *= dim;
    COLOR = c;
}
