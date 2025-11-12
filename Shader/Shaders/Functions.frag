#version 330 core
out vec4 FragColor;
uniform float iTime;
uniform vec2 iResolution;
uniform vec2 iMouse;
uniform float iMouseClick;

uniform sampler2D uAccumTex;  // textura onde os cliques foram “pintados”


// Funcao para desenhar circulo com borda suave
vec4 DrawCircle(vec2 uv, vec3 color, vec2 pos, float radius, float edge) {
    vec2 posn = pos / iResolution.xy;
    float dist = distance(uv, posn);
    float alpha = smoothstep(radius, radius - edge, dist);
    return vec4(color, alpha);
}

void main()
{
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    vec2 mouseNorm = iMouse.xy / iResolution.xy;
    vec3 baseColor = 0.5 + 0.5 * cos(iTime + uv.xyx + vec3(0,2,4));
    float trail = 0.0;

    // Aumenta o ganho e ajusta a posição do mouse
    for (float t = 0.0; t < 1.0; t += 0.01) {
        vec2 pos = mix(mouseNorm, mouseNorm + vec2(sin(iTime), cos(iTime)) * 0.3, t);
        trail += smoothstep(0.03, 0.0, distance(uv, pos));
    }
    trail *= 2.0; // ganho para destacar o rastro

    // Aplica o rastro como cor azul
    vec3 trailColor = vec3(0.0, 0.0, trail);
    FragColor = vec4(baseColor + trailColor, 1.0);
}



