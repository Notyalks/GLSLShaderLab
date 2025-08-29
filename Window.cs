using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
 
namespace GLSLShaderLab
{
    public class Window : GameWindow
    {
        private int _vao;
        private Shader _shader;
        private float _time;
        private ShaderSelector.ShaderInfo _selectedShader;
        private List<ShaderSelector.ShaderInfo> _availableShaders;
        private int _currentShaderIndex;
        private bool _showHelp;
 
        private int _clickCount = 0;          
        private bool _wasMouseDown = false;  
 
        public Window(int width, int height, string title, ShaderSelector.ShaderInfo selectedShader)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = new Vector2i(width, height), Title = $"{title} - {selectedShader.Name}" })
        {
            _selectedShader = selectedShader;
 
            var selector = new ShaderSelector();
            _availableShaders = selector.GetAvailableShaders();
            _currentShaderIndex = _availableShaders.FindIndex(s => s.Name == selectedShader.Name);
            if (_currentShaderIndex == -1) _currentShaderIndex = 0;
        }
 
        protected override void OnLoad()
        {
            base.OnLoad();
 
            float[] vertices = {
                -1f, -1f, 0f,
                 1f, -1f, 0f,
                 1f,  1f, 0f,
                -1f,  1f, 0f,
            };
 
            uint[] indices = {
                0, 1, 2,
                2, 3, 0
            };
 
            _vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();
            int ebo = GL.GenBuffer();
 
            GL.BindVertexArray(_vao);
 
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
 
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
 
            LoadShader(_selectedShader);
 
            Console.WriteLine("Controles:");
            Console.WriteLine("  setas: Trocar entre shaders");
            Console.WriteLine("  H   : Mostrar/ocultar ajuda");
            Console.WriteLine("  ESC : Sair");
            Console.WriteLine();
        }
 
        private void LoadShader(ShaderSelector.ShaderInfo shaderInfo)
        {
            try
            {
                _shader?.Dispose();
                _shader = new Shader(shaderInfo.VertexPath, shaderInfo.FragmentPath);
                _shader.Use();
                Title = $"GLSL Shader Lab - {shaderInfo.Name}";
                Console.WriteLine($"Shader carregado: {shaderInfo.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar shader {shaderInfo.Name}: {ex.Message}");
            }
        }
 
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
 
            switch (e.Key)
            {
                case Keys.Escape:
                    Close();
                    break;
 
                case Keys.Left:
                    if (_availableShaders.Count > 1)
                    {
                        _currentShaderIndex = (_currentShaderIndex - 1 + _availableShaders.Count) % _availableShaders.Count;
                        LoadShader(_availableShaders[_currentShaderIndex]);
                    }
                    break;
 
                case Keys.Right:
                    if (_availableShaders.Count > 1)
                    {
                        _currentShaderIndex = (_currentShaderIndex + 1) % _availableShaders.Count;
                        LoadShader(_availableShaders[_currentShaderIndex]);
                    }
                    break;
 
                case Keys.H:
                    _showHelp = !_showHelp;
                    break;
            }
        }
 
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
 
            bool isMouseDown = MouseState.IsButtonDown(MouseButton.Left);
            if (isMouseDown && !_wasMouseDown)
            {
                _clickCount++;
            }
            _wasMouseDown = isMouseDown;
        }
 
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            _time += (float)args.Time;
 
            GL.Clear(ClearBufferMask.ColorBufferBit);
 
            _shader?.Use();
            _shader?.SetFloat("iTime", _time);
            _shader?.SetVector2("iResolution", new Vector2(Size.X, Size.Y));
            _shader?.SetVector2("iMouse", new Vector2(MouseState.X, Size.Y - MouseState.Y));
            _shader?.SetFloat("iMouseClick", MouseState.IsButtonDown(MouseButton.Left) ? 1 : 0);
            _shader?.SetFloat("iClickCount", (float)_clickCount);  // Passa o número de cliques para o shader
 
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
 
            SwapBuffers();
        }
 
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
 
        protected override void OnUnload()
        {
            _shader?.Dispose();
            GL.DeleteVertexArray(_vao);
            base.OnUnload();
        }
    }
}