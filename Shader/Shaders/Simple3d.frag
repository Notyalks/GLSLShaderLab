#version 330 core
out vec4 FragColor;
 
in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;
in vec3 WorldPos;
 
uniform float iTime;
uniform vec2 iResolution;
uniform vec3 viewPos;
uniform sampler2D texture0;
uniform sampler2D texture1;
uniform sampler2D texture2;
uniform sampler2D texture3;
 
vec3 lightDir = vec3(0.2,-1.0,-0.2);
float ambient = 0.2;
 
void main()
{
    float brightness = clamp(dot(Normal, -lightDir), 0.0, 1.0);
    vec4 texColor = texture(texture0, TexCoord); 
    vec4 texColor1 = texture(texture1, TexCoord); 
    vec4 texColor2 = texture(texture2, TexCoord); 
    vec4 texColor3 = texture(texture3, TexCoord); 
 
    FragColor = texColor * (brightness + ambient);
 
    FragColor = mix(texColor2, texColor3, 1 - cos(brightness) ) * (brightness + ambient);
}