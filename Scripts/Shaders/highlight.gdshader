shader_type canvas_item;

uniform float outline_size : hint_range(0, 10) = 3.0;
uniform vec4 outline_color : source_color = vec4(1.0, 0.8, 0.2, 1.0);

void fragment() {
    vec4 original = texture(TEXTURE, UV);
    vec4 final_color = original;

    // Process outline only for transparent pixels
    if (original.a <= 0.1) {
        // Check surrounding pixels for non-transparent ones
        float outline_alpha = 0.0;
        float distance = 100.0;

        for (float x = -outline_size; x <= outline_size; x += 1.0) {
            for (float y = -outline_size; y <= outline_size; y += 1.0) {
                vec2 offset = vec2(x, y) * TEXTURE_PIXEL_SIZE;
                float curr_distance = length(offset);

                // Skip if outside outline size
                if (curr_distance > outline_size * length(TEXTURE_PIXEL_SIZE)) {
                    continue;
                }

                vec4 neighbor = texture(TEXTURE, UV + offset);

                if (neighbor.a > 0.1) {
                    outline_alpha = 1.0;
                    distance = min(distance, curr_distance);
                }
            }
        }

        // Apply outline with falloff based on distance
        if (outline_alpha > 0.0) {
            float falloff = 1.0 - (distance / (outline_size * length(TEXTURE_PIXEL_SIZE)));
            final_color = vec4(outline_color.rgb, outline_color.a * falloff * falloff);
        } else {
            final_color = vec4(0.0);
        }
    }

    COLOR = final_color;
}
