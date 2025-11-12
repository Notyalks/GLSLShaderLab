#version 330 core
out vec4 FragColor;

uniform vec2  iResolution;
uniform vec2  iMouse;        // pixels
uniform int   iMouseClick;   // 0 or 1
uniform sampler2D iChannel0; // previous frame
uniform float iTime;
// params
const float SCALE  = 1.001;  // >1.0 = expand outward (zoom-in)
const float RADIUS = 0.03;   // brush radius (normalized)
const float EDGE   = 0.02;   // brush soft edge (normalized)

// soft circle mask in [0..1], 1 at center
float softCircle(vec2 uvN, vec2 centerN, float r, float edge) {
    float d = distance(uvN, centerN);
    return 1.0 - smoothstep(r - edge, r, d);
}

void main() {
    vec2 uvN = gl_FragCoord.xy / iResolution.xy;

    // fixed center (stable). If you want from mouse: center = iMouse / iResolution;
    //vec2 center = vec2(0.5, 0.5);
    vec2 center = iMouse / iResolution;
    // IMPORTANT: use inverse scale for zoom-in (expand outward)
    float invScale = 1.0 / SCALE;
    vec2 scaledUV = center + (uvN - center) * invScale;

    // sample both: scaled and unscaled
    vec3 colScaled   = texture(iChannel0, clamp(scaledUV, 0.0, 1.0)).rgb;
    vec3 colUnscaled = texture(iChannel0, uvN).rgb;

    // if scaledUV is outside [0,1], fall back to unscaled (prevents streaks)
    vec2 ok = step(vec2(0.0), scaledUV) * step(scaledUV, vec2(1.0));
    float inside = ok.x * ok.y;
    vec3 color = mix(colUnscaled, colScaled, inside);

    // paint (seed) — expands due to feedback
    if (iMouseClick == 1) {
        vec2 mN = iMouse / iResolution;
        float a = softCircle(uvN, mN, RADIUS, EDGE);
        vec3 paint = 0.5 + 0.5 * cos(iTime + uvN.xyx + vec3(0,2,4));
        color = mix(color, paint, a);
    }

    color *= 0.99;

    FragColor = vec4(color, 1.0);
}
