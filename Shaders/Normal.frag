#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;
in vec3 WorldPos;

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 viewPos;
uniform sampler2D texture2;
uniform sampler2D texture1;
uniform sampler2D texture0; 

vec3 lightDir = vec3(1.2,-1.0,-0.2);
float ambient = 0.2;

void main()
{
    // Amostra o normal do normal map (em espaço de textura)
    vec3 normalMap = texture(texture2, TexCoord).rgb;
    // Converte de [0,1] para [-1,1]
    normalMap = normalize(normalMap * 2.0 - 1.0);

    // Mistura o normal original com o normal do normal map
    vec3 finalNormal = normalize(mix(Normal, normalMap, 0.7));

    float brightness = clamp(dot(finalNormal, -lightDir), 0.0, 1.0);

    vec4 texColor = texture(texture1, TexCoord);
    vec4 texColor1 = texture(texture0, TexCoord);

    FragColor = texColor * (brightness + ambient);
    
   
    
}
