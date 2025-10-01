#version 330 core
out vec4 fragColor;
uniform float iTime;
uniform vec2 iResolution;
in vec2 TexCoord;
in vec3 Normal;

vec3 lightDir = vec3(0.75, -1.0, 0.0);
float ambient = 0.2;

void main()
{
    vec2 uv = TexCoord;
    
    uv -= 0.5;
    uv.x *= iResolution.x / iResolution.y;

    vec3 col = vec3(0.0);

    // Fundo verde
    col = vec3(0.0, 0.5, 0.2);

    // Losango amarelo
    vec2 p = uv;
    float d = abs(p.x * 0.6) + abs(p.y);
    if(d < 0.45) {
        col = vec3(1.0, 0.85, 0.0);
    }

    // C�rculo azul
    float circle = length(p);
    if(circle < 0.2) {
        col = vec3(0.0, 0.2, 0.6);
    }

    // Faixa branca
    if(abs(p.y + p.x * 0.2) < 0.05 && circle < 0.2) {
        col = vec3(1.0);
    }

    float brightness = clamp(dot(Normal, -lightDir), 0.0, 1.0);
 
    fragColor = vec4(col, 1.0) * (brightness + ambient);
}
