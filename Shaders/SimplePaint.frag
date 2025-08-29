#version 330 core
out vec4 FragColor;
uniform float iTime;
uniform vec2 iResolution;
uniform vec2 iMouse;
uniform int iMouseClick;

// Buffer for persistence (previous frame)
uniform sampler2D iChannel0;

void main()
{
    // Normalized coordinates (0 to 1)
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    
    // Normalized mouse position
    vec2 mousePos = iMouse.xy / iResolution.xy;
    
    // Read previous frame for persistence
    vec3 previousFrame = texture(iChannel0, uv).rgb;
    
    // Gradient background color
    vec3 backgroundColor = mix(
        vec3(0.2, 0.1, 0.4), // dark purple
        vec3(0.1, 0.3, 0.6), // dark blue
        uv.y
    );
    
    // Start with previous frame if exists, otherwise background
    vec3 finalColor = length(previousFrame) > 0.01 ? previousFrame : backgroundColor;
    
    // Add stars in background (only if no previous content)
    if (length(previousFrame) < 0.01) {
        vec2 starUV = uv * 20.0;
        vec2 starId = floor(starUV);
        vec2 starLocal = fract(starUV) - 0.5;
        
        float starNoise = fract(sin(dot(starId, vec2(12.9898, 78.233))) * 43758.5453);
        if (starNoise > 0.98) {
            float starDist = length(starLocal);
            float star = 1.0 - smoothstep(0.0, 0.1, starDist);
            finalColor += vec3(1.0) * star * 0.3;
        }
    }
    
    // Always show small indicator where mouse is
    float mouseDistance = distance(uv, mousePos);
    float mouseIndicator = 1.0 - smoothstep(0.005, 0.015, mouseDistance);
    finalColor = mix(finalColor, vec3(1.0, 1.0, 0.0), mouseIndicator * 0.7);
    
    // If mouse is clicked, draw larger circle with persistence
    if (iMouseClick == 1) {
        float paintDistance = distance(uv, mousePos);
        float paintCircle = 1.0 - smoothstep(0.05, 0.08, paintDistance);
        
        // Animated brush color with time
        vec3 brushColor = vec3(
            0.5 + 0.5 * sin(iTime * 2.0),
            0.7,
            0.5 + 0.5 * cos(iTime * 1.5)
        );
        
        // Paint with persistence
        finalColor = mix(finalColor, brushColor, paintCircle * 0.9);
    }
    
    // Optional: very slow fade (uncomment for gradual fade)
    // finalColor *= 0.995;
    
    FragColor = vec4(finalColor, 1.0);
}