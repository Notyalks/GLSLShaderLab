#version 330 core
out vec4 FragColor;

uniform vec2 iResolution;
uniform vec2 iMouse;
uniform int iMouseClick;
uniform sampler2D iChannel0;
uniform float iTime;
const float SCALE = 0.99;
const float RADIUS = 0.03;
const float EDGE = 0.02;

float softCircle(vec2 uvN, vec2 centerN, float r, float edge) {
    float d = distance(uvN, centerN);
    return 1.0 - smoothstep(r - edge, r, d);
}

void main() {
    vec2 uvN = gl_FragCoord.xy / iResolution.xy;

    vec2 center = iMouse / iResolution;
    
    float invScale = 1.001 / SCALE;
    vec2 scaledUV = center + (uvN - center) * invScale;
    
    vec3 colPrev = texture(iChannel0, clamp(scaledUV, 0.0, 1.0)).rgb;

    vec3 procedural_color = 0.5 + 0.5 * cos(iTime + uvN.xyx * 10.0 + vec3(0,2,4));

    vec3 color = mix(procedural_color, colPrev, 0.99);

    if (iMouseClick == 1) {
        vec2 mN = iMouse / iResolution;
        float a = softCircle(uvN, mN, RADIUS, EDGE);
        vec3 paint = vec3(1.0, 0.0, 0.5);
        color = mix(color, paint, a);
    }
    
    FragColor = vec4(color, 1.0);
}