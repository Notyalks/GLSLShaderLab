#version 330 core
out vec4 fragColor;
uniform float iTime;
uniform vec2 iResolution;
in vec2 TexCoord;

void main()
{
    //vec2 uv = gl_FragCoord.xy / iResolution.xy;
    vec2 uv = TexCoord;
    
    if(uv.y > 0.6670 && uv.y < 0.7777 && uv.x < 0.3333)
    {
        fragColor = vec4(1.0,1.0,1.0,1.0);
    }
    else if(uv.x > 0.0999 && uv.x < 0.1888 && uv.y > 0.4)
    {
        fragColor = vec4(1.0,1.0,1.0,1.0);
    }
    else if (uv.y > 0.7777 && uv.y < 0.8888 && uv.x > 0.3333)
    {
        fragColor = vec4(1.0,1.0,1.0,1.0);
    }
    else if (uv.y > 0.5555 && uv.y < 0.6666 && uv.x > 0.3333)
    {
        fragColor = vec4(1.0,1.0,1.0,1.0);
    }
    else if (uv.y > 0.3333 && uv.y < 0.4444)
    {
        fragColor = vec4(1.0,1.0,1.0,1.0);
    }
    else if (uv.y > 0.1111 && uv.y < 0.2222)
    {
        fragColor = vec4(1.0,1.0,1.0,1.0);
    }
    else 
    {
        fragColor = vec4(0.02,0.369,0.694,1.0);
    }
}
