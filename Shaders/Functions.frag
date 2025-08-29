#version 330 core
out vec4 FragColor;
 
uniform float iTime;
uniform vec2 iResolution;
uniform vec2 iMouse;
uniform float iMouseClick;
uniform float iClickCount;
 
float CircleDist(vec2 uv, vec2 pos) {
    return distance(uv, pos);
}
 
float SquareDist(vec2 uv, vec2 pos) {
    vec2 d = abs(uv - pos) - vec2(0.1);
    return max(d.x, d.y);
}
 
float EquilateralTriangleDist(vec2 uv, vec2 pos) {
    vec2 p = uv - pos;
    p /= 0.1;
 
    const float k = sqrt(3.0);
    p.x = abs(p.x) - 1.0;
    p.y = p.y + 1.0 / k;
    if (p.x + k * p.y > 0.0)
        p = vec2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
    p.x -= clamp(p.x, -2.0, 0.0);
    return -length(p) * sign(p.y);
}
 
// Forma estrela: distância baseada em raio e angulo
float StarDist(vec2 uv, vec2 pos) {
    vec2 p = uv - pos;
    p /= 0.1;
    float angle = atan(p.y, p.x);
    float radius = length(p);
    float spikes = 5.0;
    float r = cos(spikes * angle) * 0.5 + 0.5;
    return radius - r;
}
 
float CubeDist(vec2 uv, vec2 pos) {
    vec2 p = uv - pos;
    return max(abs(p.x), abs(p.y)) - 0.1;
}
 
void main()
{
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    vec3 baseColor = 0.5 + 0.5 * cos(iTime + uv.xyx + vec3(0, 2, 4));
 
    vec2 mouseNorm = iMouse / iResolution;
 
    int shapeType = int(mod(iClickCount, 5.0));
    float edge = 0.01;
    float outlineWidth = 0.005;
 
    float dist = 1.0; // inicial distante do shape (fora da forma)
    vec3 shapeColor = vec3(0.0);
    bool drawShape = (iMouseClick > 0.5);
 
    if(drawShape) {
        if (shapeType == 0) dist = CircleDist(uv, mouseNorm);
        else if (shapeType == 1) dist = EquilateralTriangleDist(uv, mouseNorm);
        else if (shapeType == 2) dist = SquareDist(uv, mouseNorm);
        else if (shapeType == 3) dist = StarDist(uv, mouseNorm);
        else if (shapeType == 4) dist = CubeDist(uv, mouseNorm);
 
        if (shapeType == 0) shapeColor = vec3(0.0, 0.0, 1.0);
        else if (shapeType == 1) shapeColor = vec3(1.0, 0.0, 0.0);
        else if (shapeType == 2) shapeColor = vec3(0.0, 1.0, 0.0);
        else if (shapeType == 3) shapeColor = vec3(1.0, 1.0, 0.0);
        else if (shapeType == 4) shapeColor = vec3(1.0, 0.0, 1.0);
    }
 
    float alpha = smoothstep(edge, edge - 0.005, dist);
    float outline = smoothstep(edge + outlineWidth, edge, dist) - smoothstep(edge, edge - 0.005, dist);
 
    vec3 outlineColor = vec3(0.0);
 
    vec3 color = baseColor;
    color = mix(color, outlineColor, outline);
    color = mix(color, shapeColor, alpha);
 
    FragColor = vec4(color, 1.0);
}

 