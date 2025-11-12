#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

uniform float iTime;
uniform vec2 iResolution;

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
    
    // Animated color based on world position and time
    vec3 animatedColor = 0.5 + 0.5 * cos(iTime + FragPos.xyz * 2.0 + vec3(0,2,4));
    
    // Combine lighting with animated color
    vec3 result = (ambient + diffuse) * animatedColor;
    
    FragColor = vec4(result, 1.0);
}
