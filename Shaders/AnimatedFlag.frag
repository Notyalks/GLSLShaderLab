#version 330 core
out vec4 fragColor;
uniform float iTime;
uniform vec2 iResolution;

void main()
{
     // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = gl_FragCoord.xy/iResolution.xy;
    
    float cost=cos(iTime+uv.y*4.0)*1.0;
  
    float fs=2.0+cost*0.8;
   
    if(uv.x>(0.1*cost+0.2)){
        
        fragColor = vec4(1.0,0,0,0)*fs;
    } 
    if(uv.x>(0.1*cost+0.4)){
        
        fragColor = vec4(0.7,0.4,0,0)*fs;
    } 
    if(uv.x>(0.1*cost+0.7)){
        
        fragColor = vec4(.5)*fs;
    } 
     //corte vertical
    if(uv.x<(0.1*cost+0.2)){
        
        fragColor = vec4(0);
    }
    //corte vertical
    if(uv.x>(0.1*cost+0.9)){
        fragColor = vec4(0);
    }
    if(uv.y<0.1){
     fragColor = vec4(0);
    }
    if(uv.y>0.9){
     fragColor = vec4(0);
    }
   
    


}
