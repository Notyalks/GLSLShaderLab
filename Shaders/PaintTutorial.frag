#version 330 core
out vec4 FragColor;
uniform float iTime;
uniform vec2 iResolution;
uniform vec2 iMouse;
uniform int iMouseClick;  // 1 when mouse is clicked, 0 when not

// Buffer to store what was painted before (REQUIRED for persistence)
uniform sampler2D iChannel0;

void main()
{
    // STEP 1: Get screen coordinates (0 to 1)
    vec2 coordenadas = gl_FragCoord.xy / iResolution.xy;
    
    // STEP 2: Get mouse position (0 to 1)
    vec2 posicaoMouse = iMouse.xy / iResolution.xy;
    
    // STEP 3: Read what was painted before (iChannel0)
    vec3 pinturaAnterior = texture(iChannel0, coordenadas).rgb;
    
    // STEP 4: Detect if buffers are working
    float temBuffer = length(pinturaAnterior) > 0.001 ? 1.0 : 0.0;
    
    // STEP 5: Base background color
    vec3 corFundo = vec3(0.1, 0.15, 0.3); // dark blue
    
    // STEP 6: Start with previous painting if it exists
    vec3 corFinal = pinturaAnterior.rgb;
    
    // If no previous painting, use background color
    if (length(corFinal) < 0.001) {
        corFinal = corFundo;
    }
    
    // STEP 7: Check if mouse is clicked
    if (iMouseClick == 1) {
        // Calculate distance from current pixel to mouse
        float distancia = distance(coordenadas, posicaoMouse);
        
        // Create a smooth circle
        float tamanhoCirculo = 0.06;
        float circulo = 1.0 - smoothstep(tamanhoCirculo - 0.015, tamanhoCirculo + 0.015, distancia);
        
        // Brush color (vibrant pink)
        vec3 corPincel = vec3(0.95, 0.3, 0.7);
        
        // PAINT: mix with what already existed
        corFinal = mix(corFinal, corPincel, circulo * 0.85);
    }
    
    // STEP 8: Visual mouse indicator (always visible)
    float distanciaIndicador = distance(coordenadas, posicaoMouse);
    float indicadorMouse = 1.0 - smoothstep(0.003, 0.008, distanciaIndicador);
    vec3 corIndicador = vec3(1.0, 1.0, 0.0); // bright yellow
    corFinal = mix(corFinal, corIndicador, indicadorMouse * 0.8);
    
    // STEP 9: Visual warning if buffers are not active
    // (temporary drawing that blinks)
    if (length(pinturaAnterior) < 0.001 && iMouseClick == 1) {
        // Blink to warn that it's not permanent
        float piscar = 0.5 + 0.5 * sin(iTime * 8.0);
        vec3 corAviso = vec3(1.0, 0.2, 0.2); // red
        
        // Add blinking border around circle
        float distanciaAviso = distance(coordenadas, posicaoMouse);
        float bordaAviso = 1.0 - smoothstep(0.08, 0.09, distanciaAviso);
        bordaAviso *= (1.0 - smoothstep(0.07, 0.08, distanciaAviso));
        
        corFinal = mix(corFinal, corAviso, bordaAviso * piscar * 0.7);
    }
    
    // STEP 10: Final result
    FragColor = vec4(corFinal, 1.0);
}