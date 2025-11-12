# GLSLShaderLab

Um laboratório interativo para experimentar com shaders GLSL em modelos 3D, perfeito para estudantes aprenderem programação de shaders com renderização tridimensional.

## 🚀 Como Usar

1. **Execute o programa**dotnet run
2. **Selecione um modelo 3D**
   - O programa irá mostrar uma lista de todos os modelos disponíveis na pasta `Mesh/`
   - Digite o número correspondente ao modelo que deseja carregar
   - Suporte para: GLB, GLTF, OBJ, FBX, DAE, 3DS

3. **Selecione um shader**
   - O programa irá mostrar uma lista de todos os shaders disponíveis
   - Digite o número correspondente ao shader que deseja testar
   - Digite 'r' para recarregar a lista caso adicione novos shaders

4. **Experimente!**
   - A janela abrirá com o modelo 3D e shader selecionados
   - Use os controles para navegar e experimentar
   - O título da janela mostrará qual modelo e shader estão sendo usados

## 🎮 Controles

### Navegação da Câmera
- **WASD** - Mover câmera (frente, trás, esquerda, direita)
- **Mouse** - Olhar ao redor (mova o mouse para rotacionar a visão)
- **Scroll** - Zoom in/out
- **R** - Resetar posição da câmera

### Troca de Conteúdo
- **Q/E** - Trocar entre shaders disponíveis
- **Z/X** - Trocar entre modelos 3D disponíveis
- **H** - Mostrar/ocultar ajuda
- **ESC** - Sair do programa

## 📁 Estrutura de Arquivos

### Shaders (`Shaders/`)
Os shaders devem estar organizados no diretório `Shaders/` e seguir a convenção:
- `nomeDoShader.vert` - Vertex shader
- `nomeDoShader.frag` - Fragment shader

### Modelos 3D (`Mesh/`)
Os modelos 3D devem estar na pasta `Mesh/` nos formatos suportados:
- `.glb` - GLTF Binary (recomendado)
- `.gltf` - GLTF Text
- `.obj` - Wavefront OBJ
- `.fbx` - Autodesk FBX
- `.dae` - Collada
- `.3ds` - 3D Studio

### Shaders Incluídos

1. **basic3d** - Shader 3D básico com iluminação especular e variação de cor animada
2. **shader** - Shader colorido animado com iluminação básica
3. **waves** - Efeito de ondas aplicado ao modelo 3D com deformação de vértices

## 🎨 Criando Seus Próprios Shaders para 3D

Para criar um novo shader 3D:

1. Crie dois arquivos no diretório `Shaders/`:
   - `meuShader.vert`
   - `meuShader.frag`

2. O vertex shader deve incluir as entradas de atributos 3D:#version 330 core
layout(location = 0) in vec3 aPos;   // Posição do vértice
layout(location = 1) in vec3 aNormal; // Normal do vértice
layout(location = 2) in vec2 aTexCoord; // Coordenadas de textura

uniform mat4 model;   // Matriz de transformação do modelo
uniform mat4 view;       // Matriz de visão da câmera
uniform mat4 projection; // Matriz de projeção

out vec3 FragPos;   // Posição do fragmento no espaço mundo
out vec3 Normal;    // Normal do fragmento
out vec2 TexCoord; // Coordenadas de textura

void main()
{
    FragPos = vec3(model * vec4(aPos, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;
    TexCoord = aTexCoord;
    
    gl_Position = projection * view * vec4(FragPos, 1.0);
   }
3. No fragment shader você tem acesso a:
   - `uniform float iTime` - Tempo em segundos
   - `uniform vec2 iResolution` - Resolução da tela
   - `uniform vec3 viewPos` - Posição da câmera
   - `in vec3 FragPos` - Posição do fragmento no espaço mundo
   - `in vec3 Normal` - Normal do fragmento
   - `in vec2 TexCoord` - Coordenadas de textura

4. Execute o programa novamente ou use 'r' para recarregar

## 🛠️ Uniforms Disponíveis

### Automaticamente Definidos
- `mat4 model` - Matriz de transformação do modelo
- `mat4 view` - Matriz de visão da câmera
- `mat4 projection` - Matriz de projeção perspectiva
- `vec3 viewPos` - Posição da câmera no espaço mundo

### Shadertoy-like
- `float iTime` - Tempo decorrido em segundos (útil para animações)
- `vec2 iResolution` - Resolução da janela

## 📚 Exemplos de Shaders 3D

### Iluminação Básica#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

uniform vec3 viewPos;

void main()
{
    vec3 norm = normalize(Normal);
    vec3 lightPos = vec3(2.0, 4.0, 2.0);
    vec3 lightDir = normalize(lightPos - FragPos);
    
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * vec3(1.0, 1.0, 1.0);
    
    vec3 ambient = 0.3 * vec3(1.0, 1.0, 1.0);
    vec3 result = (ambient + diffuse) * vec3(0.6, 0.8, 1.0);
    
    FragColor = vec4(result, 1.0);
}
### Deformação com Ondas (Vertex Shader)#version 330 core
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
    vec3 pos = aPos;
    
    // Adicionar deformação de onda
    pos.y += sin(pos.x * 10.0 + iTime * 2.0) * 0.1;
    pos.y += sin(pos.z * 15.0 + iTime * 3.0) * 0.05;
    
    FragPos = vec3(model * vec4(pos, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;
    TexCoord = aTexCoord;
    
    gl_Position = projection * view * vec4(FragPos, 1.0);
}
## 🔧 Requisitos

- .NET 7 ou superior
- OpenTK 4.7.6
- AssimpNet 5.0.0-beta1 (para carregamento de modelos 3D)

## 🎯 Dicas para Estudantes

1. **Comece com modelos simples** - Use modelos básicos como cubos ou esferas
2. **Experimente com iluminação** - Adicione múltiplas luzes e diferentes tipos
3. **Use deformação de vértices** - Modifique posições no vertex shader para efeitos interessantes
4. **Combine matemática e arte** - Use funções trigonométricas para criar padrões
5. **Teste em tempo real** - Use Q/E e Z/X para trocar rapidamente entre shaders e modelos
