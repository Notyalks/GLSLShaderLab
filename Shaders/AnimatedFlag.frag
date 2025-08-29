#version 330 core
out vec4 fragColor;
uniform float iTime;
uniform vec2 iResolution;

void main()
{
     // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = gl_FragCoord.xy/iResolution.xy;
    
    // waveform to create a motion on the lines 
    float wave=cos(iTime + uv.x * 4.0) * 0.8;
    
    float fs=2.0 + wave * 1.8;
   
    //horizontal lines 
    if(uv.y > (0.1*wave+0.2)){
        
        fragColor = vec4(0.7,0,0,0) * fs;
    } 
    if(uv.y > (0.1* wave + 0.4)){
        
        fragColor = vec4(0.7,0.4,0,0) * fs;
    } 
    if(uv.y > (0.1 * wave + 0.7)){
        
        fragColor = vec4(0.5) * fs;
    } 
     //corte vertical
    if(uv.y < (0.1 * wave + 0.2)){
        
        fragColor = vec4(0);
    }

    //vertical cut 
    if(uv.y > (0.1 * wave + 0.9)){
        fragColor = vec4(0);
    }
    if(uv.x < 0.1){
     fragColor = vec4(0);
    }
    if(uv.x > 0.9){
     fragColor = vec4(0);
    }
}
