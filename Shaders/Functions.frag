#version 330 core
out vec4 FragColor;
uniform float iTime;
uniform vec2 iResolution;
uniform vec2 iMouse;
uniform int iMouseClick;  // 1 quando o mouse esta clicado, 0 quando nao

// Input buffers (iChannels)
uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform sampler2D iChannel3;

// Function to draw circle with smooth border
vec4 DrawCircle(vec2 uv, vec3 color, vec2 pos, float radius, float edge) {
    vec2 posn = pos / iResolution.xy;
    float dist = distance(uv, posn);
    float alpha = smoothstep(radius, radius - edge, dist);
    return vec4(color, alpha);
}

// Function to apply feedback using previous buffers
vec4 ApplyFeedback(vec2 uv, vec4 currentColor) {
    // Read previous frame from iChannel0
    vec4 previousFrame = texture(iChannel0, uv);
    
    // Apply small displacement to create trails
    vec2 offset = vec2(sin(iTime * 0.5) * 0.001, cos(iTime * 0.3) * 0.001);
    vec4 feedbackColor = texture(iChannel0, uv + offset);
    
    // Mix current frame with feedback, creating trails
    return mix(currentColor, feedbackColor * 0.95, 0.3);
}

void main()
{
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    vec2 mouseNorm = iMouse.xy / iResolution.xy;
    
    // Animated base color
    vec3 baseColor = 0.5 + 0.5 * cos(iTime + uv.xyx + vec3(0,2,4));
    
    // Draw circle at mouse position only when clicking
    vec4 circle = vec4(0.0);
    if (iMouseClick == 1) {
        circle = DrawCircle(uv, vec3(0.0, 0.0, 1.0), iMouse.xy, 0.1, 0.01);
    }
    
    // Current color without feedback
    vec4 currentColor = mix(vec4(baseColor, 1.0), circle, circle.a);
    
    // Apply feedback using previous buffers
    vec4 finalColor = ApplyFeedback(uv, currentColor);
    
    // Example usage of other channels
    // iChannel1 can be used for additional textures
    vec4 texture1 = texture(iChannel1, uv * 2.0);
    
    // iChannel2 can be used for noise or other effects
    vec4 texture2 = texture(iChannel2, uv + iTime * 0.1);
    
    // Subtle mix of additional channels
    finalColor.rgb += texture1.rgb * 0.1 + texture2.rgb * 0.05;
    
    FragColor = finalColor;
}


