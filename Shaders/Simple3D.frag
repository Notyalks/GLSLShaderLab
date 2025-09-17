#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;
in vec3 WorldPos;

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 viewPos;

void main()
{
    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
}