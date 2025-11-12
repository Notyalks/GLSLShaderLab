#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform float iTime;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoord;

void main()
{
    // Apply wave displacement to the vertices
    vec3 pos = aPos;
    
    // Add wave deformation based on position and time
    float wave1 = sin(pos.x * 10.0 + iTime * 2.0) * 0.1;
    float wave2 = sin(pos.z * 15.0 + iTime * 3.0) * 0.05;
    float wave3 = sin(pos.x * 8.0 + pos.z * 6.0 + iTime * 1.5) * 0.08;
    
    pos.y += wave1 + wave2 + wave3;
    
    FragPos = vec3(model * vec4(pos, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;
    TexCoord = aTexCoord;
    
    gl_Position = projection * view * vec4(FragPos, 1.0);
}