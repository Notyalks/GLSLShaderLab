#version 330 core
out vec4 fragColor;
uniform vec2 iResolution;
in vec2 TexCoord;

void main()
{
    vec2 uv = TexCoord;
    
    // Faixa branca
    if(uv.y > 2.0/3.0) {
        fragColor = vec4(1.0, 0.0, 0.0, 1.0);
    }
    // Faixa azul
    else if(uv.y > 1.0/3.0) {
        fragColor = vec4(1.0, 1.0, 1.0, 1.0);
    }
    // Faixa vermelha
    else {
        fragColor = vec4(1.0, 0.0, 0.0, 1.0);
    }
}
