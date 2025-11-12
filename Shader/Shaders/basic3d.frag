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
    // Normalize the normal vector
    vec3 norm = normalize(Normal);
    
    // Simple lighting setup
    vec3 lightPos = vec3(2.0 * sin(iTime), 4.0, 2.0 * cos(iTime));
    vec3 lightColor = vec3(1.0, 1.0, 1.0);
    
    // Ambient lighting
    float ambientStrength = 0.3;
    vec3 ambient = ambientStrength * lightColor;
    
    // Diffuse lighting
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;
    
    // Specular lighting
    float specularStrength = 0.5;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColor;
    
    // Base color with some variation
    vec3 baseColor = vec3(0.6, 0.8, 1.0);
    
    // Add some animated color variation based on position
    float colorVariation = 0.5 + 0.5 * sin(WorldPos.x * 2.0 + iTime);
    baseColor.r += colorVariation * 0.3;
    baseColor.g -= colorVariation * 0.1;
    
    // Combine lighting
    vec3 result = (ambient + diffuse + specular) * baseColor;
    
    FragColor = vec4(result, 1.0);
}