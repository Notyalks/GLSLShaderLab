#version 330 core
out vec4 FragColor;

uniform vec2  iResolution;
uniform vec2  iMouse;
uniform int   iMouseClick;
uniform sampler2D iChannel0; 


const float SCALE   = 1.010; 
const float DAMPING = 0.996; 

float softCircle(vec2 uv, vec2 c, float r) {
    float d = distance(uv, c);
    return smoothstep(r, r*0.6, r - d);
}

void main() {
    vec2 uv = gl_FragCoord.xy / iResolution;

    
    vec4 prev = texture(iChannel0, uv);
    vec3 colorPrev = prev.rgb;
    vec2 centerPrev = prev.ba; 


    if (centerPrev == vec2(0.0)) centerPrev = vec2(0.5);


    vec2 mouse = iMouse / iResolution;
    vec2 center = (iMouseClick == 1) ? mouse : centerPrev;

   
    vec2 scaledUV = center + (uv - center) * SCALE;
    scaledUV = clamp(scaledUV, 0.0, 1.0);

    
    vec3 color = texture(iChannel0, scaledUV).rgb;

   
    color *= DAMPING;

    
    if (iMouseClick == 1) {
        float a = softCircle(uv, mouse, 0.05);
        vec3 paint = vec3(1.0, 0.5, 0.0);
        color = mix(color, paint, a);
    }

    
    FragColor = vec4(color.rg, center.x, center.y);
}
