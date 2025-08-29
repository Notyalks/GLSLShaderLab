#version 330 core
out vec4 FragColor;
uniform float iTime;
uniform vec2 iResolution;

void main()
{
    // Convert from pixel coordinates to normalized coordinates
    vec2 uv = gl_FragCoord.xy / iResolution.xy;

    // Calclulate the aspect ratio
    float prop=iResolution.x/iResolution.y;

    // Adjust the UV coordinates to maintain aspect ratio and center the pattern
    float x=  length(vec2(uv.x*prop,uv.y)-vec2(prop/2.0,0.5));

    // white background
    FragColor = vec4(1,1,1, 1.0);

    // red sprehe in center and 0.3 radius of the sphere
    if(x < 0.3) {
        FragColor = vec4(1,0,0, 1.0);
    }
   
}
