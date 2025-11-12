# ?? Troubleshooting - Shader Lab

## ? Problema: Tela Preta no PaintTutorial

### ?? **Soluções:**

#### **1. Ativar Sistema de Buffers**
- **Pressione a tecla 'B'** para ativar buffers
- Verifique se o título da janela mostra **"[Buffers ON]"**
- O PaintTutorial precisa dos buffers para funcionar corretamente

#### **2. Usar SimplePaint (sem buffers)**
- Selecione o shader **"SimplePaint"** na lista
- Funciona sem precisar ativar buffers
- Ideal para demonstrações rápidas

#### **3. Verificar Console de Erros**
- Olhe o console do Visual Studio para erros de shader
- Erros comuns:
  ```
  Fragment Shader Log: ERROR: uniform 'iMouseClick' not found
  ```

### ?? **Como Testar se Está Funcionando:**

1. **Execute o programa**
2. **Selecione "PaintTutorial"** ou **"SimplePaint"**
3. **Para PaintTutorial**: Pressione **'B'** para ativar buffers
4. **Mova o mouse** - deve ver um pontinho amarelo
5. **Clique e arraste** - deve pintar círculos coloridos

### ?? **Problemas Comuns:**

#### **Tela Completamente Preta:**
- ? Buffers não ativados no PaintTutorial
- ? Erro de compilação do shader
- ? Uniform não encontrado

#### **Mouse não Detectado:**
- ? `iMouseClick` não implementado
- ? Coordenadas do mouse invertidas
- ? `iMouse` não sendo enviado

#### **Círculos não Aparecem:**
- ? `iMouseClick` sempre 0
- ? Tamanho do círculo muito pequeno
- ? Cor do pincel igual ao fundo

### ?? **Debug Steps:**

#### **1. Teste Shader Básico:**
```glsl
void main() {
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    FragColor = vec4(uv, 0.5, 1.0); // Gradiente simples
}
```

#### **2. Teste Mouse:**
```glsl
void main() {
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
    vec2 mouse = iMouse.xy / iResolution.xy;
    float dist = distance(uv, mouse);
    float circle = 1.0 - smoothstep(0.05, 0.1, dist);
    FragColor = vec4(vec3(circle), 1.0);
}
```

#### **3. Teste Click:**
```glsl
void main() {
    vec3 color = iMouseClick == 1 ? vec3(1.0, 0.0, 0.0) : vec3(0.0, 0.0, 1.0);
    FragColor = vec4(color, 1.0);
}
```

### ?? **Shaders Recomendados por Nível:**

#### **Iniciante (sem buffers):**
- ? **SimplePaint** - Funciona imediatamente
- ? **shader** - Gradiente básico
- ? **Japao** - Padrões simples

#### **Intermediário (com buffers):**
- ? **PaintTutorial** - Sistema de pintura
- ? **Functions** - Feedback loops
- ? **waves** - Animações

#### **Avançado:**
- ? **BufferDemo** - Efeitos complexos
- ? **Clouds** - Noise procedural
- ? **AnimatedFlag** - Simulação física

### ?? **Checklist de Verificação:**

- [ ] Shader compila sem erros
- [ ] Console não mostra erros
- [ ] Buffers ativados (se necessário)
- [ ] Mouse move e clica
- [ ] Uniforms sendo enviados
- [ ] Coordenadas corretas (0-1)

### ?? **Dicas para Professores:**

1. **Sempre comece com SimplePaint** para demonstrações
2. **Explique o conceito de buffers** antes de usar PaintTutorial
3. **Mostre o console** para debug em tempo real
4. **Use cores contrastantes** para facilitar visualização
5. **Teste em diferentes resoluções** de tela

---
**Lembre-se**: Se um shader não funciona, sempre verifique primeiro se os buffers estão ativados (tecla 'B')! ??

## ? Problema: Desenho Não Permanece no PaintTutorial

### ?? **Causa Principal:**
O **PaintTutorial** precisa do **sistema de buffers ativado** para que a pintura fique permanente na tela. Sem buffers, o desenho desaparece a cada frame.

### ? **Soluções:**

#### **1. Auto-Ativação (Automática)**
- O sistema agora **ativa buffers automaticamente** para PaintTutorial
- Mensagem no console: `"Buffer system AUTO-ENABLED"`
- ? **Recomendado**: Deixe o sistema ativar automaticamente

#### **2. Ativação Manual**
- **Pressione a tecla 'B'** para ativar buffers manualmente
- Verifique se o título mostra **"[Buffers ON]"**
- Título da janela deve mostrar status dos buffers

#### **3. Indicadores Visuais**
- **Ponto amarelo** = posição do mouse (sempre visível)
- **Círculo rosa** = pintura normal (com buffers)
- **Borda vermelha piscante** = aviso que não está permanente

### ?? **Como Testar se Está Funcionando:**

#### **? COM Buffers (funcionando):**
1. Execute o programa
2. Selecione **"PaintTutorial"**
3. Buffers ativam automaticamente
4. Clique e arraste - **pintura fica permanente**
5. **SEM borda vermelha piscante**

#### **? SEM Buffers (temporário):**
1. Execute o programa
2. Selecione **"PaintTutorial"**
3. Desative buffers (tecla 'B')
4. Clique e arraste - **pintura desaparece**
5. **COM borda vermelha piscante** (aviso)

### ?? **Comparação de Comportamento:**

| Situação | Título da Janela | Pintura | Indicador |
|----------|------------------|---------|-----------|
| **Buffers ON** | `[Buffers ON]` | ? Permanente | ?? Ponto amarelo |
| **Buffers OFF** | `[Buffers OFF]` | ? Temporária | ?? Borda piscante |

### ?? **Shaders por Categoria:**

#### **?? Auto-Ativam Buffers:**
- ? **PaintTutorial** - Sistema de pintura persistente
- ? **BufferDemo** - Efeitos avançados com feedback
- ? **Functions** - Circles com trails

#### **?? Opcionais (funcionam sem):**
- ? **SimplePaint** - Pintura temporária
- ? **waves** - Animações simples
- ? **Clouds** - Padrões procedurais

#### **?? Não Precisam:**
- ? **shader** - Gradiente básico
- ? **Japao** - Bandeira estática
- ? **Grece** - Padrões fixos

### ?? **Problemas Comuns e Soluções:**

#### **"Tela Azul, Mas Desenho Não Fica"**
- **Causa**: Buffers desativados
- **Solução**: Recarregue o shader (auto-ativa) ou pressione 'B'

#### **"Borda Vermelha Piscando"**
- **Causa**: Aviso visual de que não há persistência
- **Solução**: Ative buffers (tecla 'B')

#### **"Não Vejo o Mouse"**
- **Causa**: Indicador muito pequeno
- **Solução**: Move o mouse - deve ver ponto amarelo brilhante

#### **"Auto-Ativação Não Funcionou"**
- **Causa**: Nome do shader mudou
- **Solução**: Ative manualmente com 'B'

### ?? **Dicas para Professores:**

1. **Explique a diferença** entre temporário vs permanente
2. **Mostre os indicadores visuais** (amarelo vs vermelho)
3. **Use SimplePaint primeiro** para demonstração rápida
4. **PaintTutorial depois** para explicar persistência
5. **Sempre mencione** que buffers são para "memória" entre frames

### ?? **Checklist de Verificação Rápida:**

- [ ] Shader é PaintTutorial?
- [ ] Título mostra "[Buffers ON]"?
- [ ] Vê ponto amarelo do mouse?
- [ ] Pintura fica após soltar mouse?
- [ ] NÃO há borda vermelha piscante?

Se todos ? = **Funcionando perfeitamente!** ??

### ?? **Para Entender Melhor:**

**Buffers** = "Memória visual" que guarda o que foi pintado
**iChannel0** = "Foto" do frame anterior
**Sem buffers** = Shader "esquece" o que pintou
**Com buffers** = Shader "lembra" e mantém a pintura

---
**Lembre-se**: PaintTutorial = Buffers Obrigatórios! O sistema agora ativa automaticamente! ??