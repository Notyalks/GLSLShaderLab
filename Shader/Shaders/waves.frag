#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

uniform float iTime;
uniform vec2 iResolution;
uniform vec3 viewPos;

void main()
{
    // Normalize the normal vector
    vec3 norm = normalize(Normal);
    
    // Simple lighting setup
    vec3 lightPos = vec3(2.0, 4.0, 2.0);
    vec3 lightColor = vec3(1.0, 1.0, 1.0);
    
    // Ambient lighting
    float ambientStrength = 0.3;
    vec3 ambient = ambientStrength * lightColor;
    
    // Diffuse lighting
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;
    
    // Animated wave color based on position and time
    float wave1 = sin(FragPos.x * 10.0 + iTime * 2.0) * 0.5 + 0.5;
    float wave2 = sin(FragPos.z * 15.0 + iTime * 3.0) * 0.5 + 0.5;
    float wave3 = sin(FragPos.x * 8.0 + FragPos.z * 6.0 + iTime * 1.5) * 0.5 + 0.5;
    
    // Color based on waves
    vec3 waveColor = vec3(
        0.2 + wave1 * 0.4,
        0.6 + wave2 * 0.3,
        1.0 - wave3 * 0.3
    );
    
    // Combine lighting with wave color
    vec3 result = (ambient + diffuse) * waveColor;
    
    FragColor = vec4(result, 1.0);
}