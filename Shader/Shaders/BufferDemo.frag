#version 330 core
out vec4 FragColor;
uniform float iTime;
uniform vec2 iResolution;
uniform vec2 iMouse;

// Input buffers (iChannels)
uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;

// Function to generate procedural noise
float noise(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

// Function to create fractal patterns
vec3 fractalPattern(vec2 uv, float time) {
    vec3 color = vec3(0.0);
    vec2 p = uv * 4.0;
    
    for(int i = 0; i < 6; i++) {
        float fi = float(i);
        p += 0.5 * sin(p.yx * (2.0 + fi) + time * (0.5 + fi * 0.2));
        color += 0.2 * cos(p.x + p.y + time + fi);
    }
    
    return color;
}

// Function to apply blur using neighbor samples
vec4 applyBlur(sampler2D tex, vec2 uv, float intensity) {
    vec4 color = vec4(0.0);
    float weight = 0.0;
    
    for(int x = -2; x <= 2; x++) {
        for(int y = -2; y <= 2; y++) {
            vec2 offset = vec2(x, y) * intensity / iResolution.xy;
            float w = exp(-0.5 * (float(x*x + y*y))) / (2.0 * 3.14159);
            color += texture(tex, uv + offset) * w;
            weight += w;
        }
    }
    
    return color / weight;
}

void main() {
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    vec2 mouseNorm = iMouse.xy / iResolution.xy;
    
    // Previous frame (feedback loop)
    vec4 prevFrame = texture(iChannel0, uv);
    
    // Apply time-based displacement to create movement
    vec2 distortion = vec2(
        sin(uv.y * 10.0 + iTime) * 0.01,
        cos(uv.x * 8.0 + iTime * 1.5) * 0.01
    );
    
    vec4 distortedPrev = texture(iChannel0, uv + distortion);
    
    // Generate new content
    vec3 newContent = fractalPattern(uv, iTime);
    
    // Add particles near mouse
    float mouseInfluence = 1.0 - smoothstep(0.0, 0.3, distance(uv, mouseNorm));
    newContent += mouseInfluence * vec3(1.0, 0.5, 0.2) * sin(iTime * 10.0);
    
    // Mix with previous frame to create trails
    vec3 trails = mix(newContent, distortedPrev.rgb * 0.95, 0.85);
    
    // Use iChannel1 for additional textures or noise
    vec4 texture1 = texture(iChannel1, uv * 2.0 + iTime * 0.1);
    trails += texture1.rgb * 0.1;
    
    // Use iChannel2 for blur effects from previous frame
    vec4 blurred = applyBlur(iChannel0, uv, 2.0);
    trails = mix(trails, blurred.rgb, 0.1);
    
    // Use iChannel3 for more effect layers
    vec4 texture3 = texture(iChannel3, uv + sin(iTime) * 0.1);
    trails += texture3.rgb * 0.05;
    
    // Apply gamma correction and saturation
    trails = pow(trails, vec3(0.8));
    trails = mix(vec3(dot(trails, vec3(0.299, 0.587, 0.114))), trails, 1.2);
    
    FragColor = vec4(trails, 1.0);
}