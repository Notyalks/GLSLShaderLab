# ?? Tutorial de Shader - Sistema de Pintura

## ?? O que este shader faz?

Este shader simula um **programa de pintura simples** onde você pode:
- Clicar e arrastar o mouse para desenhar círculos coloridos
- Os círculos ficam "marcados" na tela (persistem entre frames)
- Usar buffers para lembrar do que foi pintado antes

## ?? Como funciona?

### Conceitos básicos:

1. **Coordenadas UV** (0 a 1):
   ```glsl
   vec2 uv = gl_FragCoord.xy / iResolution.xy;
   ```
   - Converte pixels da tela para coordenadas normalizadas
   - (0,0) = canto inferior esquerdo
   - (1,1) = canto superior direito

2. **Detecção do Mouse Clicado**:
   ```glsl
   uniform int iMouseClick;  // 1 = clicado, 0 = não clicado
   if (iMouseClick == 1) // Mouse pressionado
   ```
   - `iMouse.xy` = posição do mouse
   - `iMouseClick` = estado do botão (0 ou 1) - mais claro que iMouse.z

3. **Função distance()**:
   ```glsl
   float dist = distance(coordenadas, posicaoMouse);
   ```
   - Calcula distância entre dois pontos
   - Usado para criar círculos

4. **Função smoothstep()**:
   ```glsl
   float circulo = 1.0 - smoothstep(tamanho - borda, tamanho, distancia);
   ```
   - Cria transições suaves (anti-aliasing)
   - Evita bordas "serrilhadas"

5. **Buffers (iChannel0)**:
   ```glsl
   vec3 anterior = texture(iChannel0, uv).rgb;
   ```
   - Lê o que foi pintado no frame anterior
   - Permite persistência da pintura

## ?? **iMouseClick vs iMouse.z**

### **Vantagens do iMouseClick:**
- ? **Mais claro**: `iMouseClick == 1` é mais legível que `iMouse.z > 0.5`
- ? **Semântica**: Nome específico para a função
- ? **Tipo correto**: `int` em vez de `float`
- ? **Pedagógico**: Mais fácil para iniciantes entenderem

### **Comparação:**
```glsl
// Método antigo (funciona, mas menos claro)
if (iMouse.z > 0.5) { /* pintar */ }

// Método novo (mais claro e didático)
if (iMouseClick == 1) { /* pintar */ }
```

## ?? Exercícios para Praticar:

### Nível Iniciante:
1. **Mudar cor do pincel**: Altere `vec3(0.9, 0.4, 0.7)` para outras cores
2. **Tamanho do círculo**: Modifique `0.08` para fazer círculos maiores/menores
3. **Cor de fundo**: Adicione uma cor de fundo fixa

### Nível Intermediário:
4. **Cores animadas**: Use `sin(iTime)` para animar as cores
5. **Múltiplos pincéis**: Diferentes cores baseadas na posição do mouse
6. **Fade gradual**: Descomente a linha do fade para pintura temporária

### Nível Avançado:
7. **Pincéis diferentes**: Quadrados, triângulos, estrelas
8. **Texturas**: Use outros iChannels para texturas de pincel
9. **Blend modes**: Experimente diferentes modos de mistura

## ?? Dicas importantes:

- **Sempre ative buffers**: Pressione 'B' no programa para ativar iChannels
- **Coordenadas**: GLSL usa (0,0) no canto inferior esquerdo
- **Performance**: Evite loops complexos em shaders de pixel
- **Debugging**: Use cores simples (vermelho, verde, azul) para testar
- **iMouseClick**: Mais intuitivo que iMouse.z para detecção de cliques

## ?? Próximos passos:

1. Entenda este shader completamente
2. Experimente modificações simples
3. Crie suas próprias funções de desenho
4. Explore outros shaders do projeto para aprender mais técnicas

---
**Lembre-se**: Shader programming é como aprender uma nova linguagem. 
Pratique muito e não tenha medo de experimentar! ??