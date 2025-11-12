#version 330 core
out vec4 o;
uniform float iTime;
uniform vec2 iResolution;

void main()
{
    // Loop and noise variables
    float i = 0.0, d = 0.0, s = 0.0, n, t = iTime * 2.0;
    // 3D vectors for position and noise calculations
    vec3 q, p = vec3(iResolution, 0.0);
    // Get fragment coordinates
    vec2 u = gl_FragCoord.xy;
    // Initialize output color
    o = vec4(0.0);

    // Center and normalize coordinates
    u = (u - p.xy / 2.0) / p.y;

    // Main marching loop for cloud layers
    for (; i++ < 100.0;) {
        // Save current position for noise and plane calculations
        q = p = vec3(u * d, d + t);

        // Noise loop to add detail to the clouds
        for (n = 0.03; n < 2.0; n += n) {
            // Modify position with sine-based noise
            p += abs(dot(sin(p * n * 4.0), vec3(0.035))) / n;
            // You could also modify q here for sky effects
        }

        // Calculate distance to two planes (ground and sky)
        d += s = 0.04 + abs(min(0.2 - q.y - (cos(p.x) * 0.2), 2.5 + p.y)) * 0.6;
        // Accumulate color based on distance
        o += vec4(1.0 / s);
    }

    // Final color normalization and tone mapping
    u -= 0.1;
    o = tanh(vec4(4, 2, 1, 1) * o / 4000.0 / length(u));
    o.a = 1.0; // Set alpha to fully opaque
}
