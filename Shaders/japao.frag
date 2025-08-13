#version 330 core
out vec4 FragColor;
uniform float iTime;
uniform vec2 iResolution;

void main()
{
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    float prop=iResolution.x /iResolution.y;

    float x= length(vec2(uv.x*prop,uv.y)-vec2(prop/2.0,0.5));

    FragColor = vec4(1,1,1, 1.0);

    if(x < 0.3){
    FragColor = vec4(1,0,0, 1.0);
    }
  
}
