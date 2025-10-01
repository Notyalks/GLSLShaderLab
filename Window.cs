using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace GLSLShaderLab
{
    public class Window : GameWindow
    {
        private Shader _shader;
        private Model _model;
        private Shader _copyShader;
        private float _time;
        private ShaderSelector.ShaderInfo _selectedShader;
        private ModelSelector.ModelInfo _selectedModel;
        private List<ShaderSelector.ShaderInfo> _availableShaders;
        private List<ModelSelector.ModelInfo> _availableModels;
        private int _currentShaderIndex;
        private int _currentModelIndex;
        private bool _showHelp;
        private BufferManager _bufferManager;
        private bool _useBuffers = false;

        // Camera properties
        private Vector3 _cameraPos = new Vector3(0.0f, 0.0f, 3.0f);
        private Vector3 _cameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        private Vector3 _cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
        private float _yaw = -90.0f;
        private float _pitch = 0.0f;
        private float _fov = 45.0f;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        // Model transformation
        private float _rotationY = 0.0f;

        // Fullscreen quad
        private int _vao;
        private int _vbo;
        private int _ebo;

        // Render mode
        private enum RenderMode { Fullscreen2D, Model3D }
        private RenderMode _renderMode = RenderMode.Fullscreen2D;

        // Textures
        private List<int> _loadedTextures = new();
        private List<string> _loadedTextureNames = new();
        private const string TextureFolderName = "textures"; // <- use plural
        private string _resolvedTextureDir = "";

        public Window(int width, int height, string title, ShaderSelector.ShaderInfo selectedShader, ModelSelector.ModelInfo selectedModel)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                Size = new Vector2i(width, height),
                Title = $"{title} - {selectedShader.Name} - {selectedModel.Name}"
            })
        {
            _selectedShader = selectedShader;
            _selectedModel = selectedModel;

            var shaderSelector = new ShaderSelector();
            var modelSelector = new ModelSelector();
            _availableShaders = shaderSelector.GetAvailableShaders();
            _availableModels = modelSelector.GetAvailableModels();

            _currentShaderIndex = _availableShaders.FindIndex(s => s.Name == selectedShader.Name);
            if (_currentShaderIndex == -1) _currentShaderIndex = 0;

            _currentModelIndex = _availableModels.FindIndex(m => m.Name == selectedModel.Name);
            if (_currentModelIndex == -1) _currentModelIndex = 0;

            CursorState = CursorState.Normal;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // Fullscreen quad
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            float[] quadVertices = {
                // pos       // texcoord
                -1.0f, -1.0f, 0.0f, 0.0f,
                 1.0f, -1.0f, 1.0f, 0.0f,
                 1.0f,  1.0f, 1.0f, 1.0f,
                -1.0f,  1.0f, 0.0f, 1.0f
            };

            uint[] quadIndices = { 0, 1, 2, 2, 3, 0 };

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, quadIndices.Length * sizeof(uint), quadIndices, BufferUsageHint.StaticDraw);

            int stride = 4 * sizeof(float);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 2 * sizeof(float));

            // Buffer manager
            _bufferManager = new BufferManager(Size.X, Size.Y);

            try { _copyShader = new Shader("Shaders/copy.vert", "Shaders/copy.frag"); }
            catch (Exception ex) { Console.WriteLine($"Erro ao carregar copy shader: {ex.Message}"); }

            LoadShader(_selectedShader);
            LoadModel(_selectedModel);

            // Resolve e carrega texturas a partir de "textures"
            _resolvedTextureDir = ResolveTexturesFolder();
            LoadTexturesFromFolder(_resolvedTextureDir, includeSubdirs: false);

            Console.WriteLine("Controles:");
            Console.WriteLine(" WASD : Mover câmera");
            Console.WriteLine(" Mouse : Olhar ao redor");
            Console.WriteLine(" Scroll : Zoom");
            Console.WriteLine(" Q/E : Trocar shaders");
            Console.WriteLine(" Z/X : Trocar modelos");
            Console.WriteLine(" R   : Resetar câmera");
            Console.WriteLine(" B   : Toggle buffer system (iChannels)");
            Console.WriteLine(" M   : Alternar modo 2D/3D");
            Console.WriteLine(" H   : Mostrar/ocultar ajuda + listar texturas");
            Console.WriteLine(" F5  : Recarregar texturas da pasta");
            Console.WriteLine(" ESC : Sair");
            Console.WriteLine($"Texturas: pasta resolvida para: {_resolvedTextureDir}");
        }

        // Resolve possíveis locais da pasta textures
        private string ResolveTexturesFolder()
        {
            var candidates = new List<string>
            {
                Path.Combine(AppContext.BaseDirectory, TextureFolderName),
                Path.Combine(Directory.GetCurrentDirectory(), TextureFolderName),
                Path.Combine(AppContext.BaseDirectory, "Assets", TextureFolderName),
                Path.Combine(AppContext.BaseDirectory, "Resources", TextureFolderName),
            };

            foreach (var c in candidates)
                if (Directory.Exists(c)) return c;

            // fallback: se não existir, retorna caminho no diretório atual
            return Path.Combine(Directory.GetCurrentDirectory(), TextureFolderName);
        }

        // Libera e limpa a lista de texturas carregadas
        private void ReleaseLoadedTextures()
        {
            foreach (var tex in _loadedTextures)
            {
                if (tex != 0) GL.DeleteTexture(tex);
            }
            _loadedTextures.Clear();
            _loadedTextureNames.Clear();
        }

        // Carrega todas as texturas da pasta especificada
        private void LoadTexturesFromFolder(string folderPath, bool includeSubdirs = false)
        {
            // Evita vazamento: libera texturas anteriores antes de recarregar
            ReleaseLoadedTextures();

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"[TEXTURE] Pasta não encontrada: {folderPath}");
                return;
            }

            var searchOption = includeSubdirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            // extensões suportadas (case-insensitive)
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".png", ".jpg", ".jpeg", ".bmp", ".tga" };

            int found = 0, ok = 0, fail = 0;

            foreach (var file in Directory.EnumerateFiles(folderPath, "*.*", searchOption))
            {
                var ext = Path.GetExtension(file);
                if (!allowed.Contains(ext)) continue;

                found++;
                int texId = LoadTexture(file);
                if (texId != 0)
                {
                    _loadedTextures.Add(texId);
                    _loadedTextureNames.Add(Path.GetFileName(file));
                    ok++;
                }
                else fail++;
            }

            Console.WriteLine($"[TEXTURE] varridos: {found}, carregados: {ok}, falhas: {fail}");
            if (ok > 0)
            {
                for (int i = 0; i < _loadedTextureNames.Count; i++)
                    Console.WriteLine($"  texture{i}: {_loadedTextureNames[i]}");
            }
            else
            {
                Console.WriteLine("[TEXTURE] Nenhuma textura carregada.");
            }
        }

        // Carrega uma textura de arquivo usando ImageSharp
        private int LoadTexture(string path)
        {
            try
            {
                using var image = Image.Load<Rgba32>(path);
                image.Mutate(x => x.Flip(FlipMode.Vertical));
                var pixels = new byte[image.Width * image.Height * 4];
                image.CopyPixelDataTo(pixels);

                int texId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texId);

                // Alinhamento para imagens sem padding
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

                // Filtros + mipmaps
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                Console.WriteLine($"[TEXTURE] Sucesso: {Path.GetFileName(path)} {image.Width}x{image.Height} (ID: {texId})");

                GL.BindTexture(TextureTarget.Texture2D, 0);
                return texId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TEXTURE] Falha: {path} - {ex.Message}");
                return 0;
            }
        }

        private void SetCommonUniforms(Shader shader)
        {
            shader.SetFloat("iTime", _time);
            shader.SetVector2("iResolution", new Vector2(Size.X, Size.Y));
            shader.SetVector2("iMouse", new Vector2(MouseState.X, Size.Y - MouseState.Y));
            shader.SetInt("iMouseClick", MouseState.IsButtonDown(MouseButton.Left) ? 1 : 0);
            shader.SetVector3("viewPos", _cameraPos);

            // Descobre quantas texture units temos e limita o bind
            GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out int maxUnits);
            int startUnit = 1; // Texture1 em diante (Texture0 reservado p/ iChannel0)
            int canBind = Math.Min(_loadedTextures.Count, Math.Max(0, maxUnits - startUnit));

            for (int i = 0; i < canBind; i++)
            {
                int unit = startUnit + i; // Texture1, 2, ...
                GL.ActiveTexture(TextureUnit.Texture0 + unit);
                GL.BindTexture(TextureTarget.Texture2D, _loadedTextures[i]);
                shader.SetInt($"texture{i}", unit);
            }
        }

        private void LoadShader(ShaderSelector.ShaderInfo shaderInfo)
        {
            try
            {
                _shader?.Dispose();
                _shader = new Shader(shaderInfo.VertexPath, shaderInfo.FragmentPath);
                _shader.Use();

                if (shaderInfo.Name.Contains("PaintTutorial") ||
                    shaderInfo.Name.Contains("BufferDemo") ||
                    shaderInfo.Name.Contains("BufferTest") ||
                    shaderInfo.Name.Contains("SimplePaint") ||
                    shaderInfo.Name.Contains("Functions"))
                {
                    if (!_useBuffers)
                    {
                        _useBuffers = true;
                        Console.WriteLine($"Buffer system AUTO-ENABLED for {shaderInfo.Name}");
                    }
                }

                Title = $"GLSL Shader Lab - {shaderInfo.Name} - Mode: {_renderMode}";
            }
            catch (Exception ex) { Console.WriteLine($"Erro ao carregar shader {shaderInfo.Name}: {ex.Message}"); }
        }

        private void LoadModel(ModelSelector.ModelInfo modelInfo)
        {
            try
            {
                _model?.Dispose();
                _model = new Model(modelInfo.FilePath);
                Title = $"GLSL Shader Lab - {_selectedShader.Name} - {modelInfo.Name} - Mode: {_renderMode}";
            }
            catch (Exception ex) { Console.WriteLine($"Erro ao carregar modelo {modelInfo.Name}: {ex.Message}"); }
        }

        private void UpdateCamera()
        {
            var front = Vector3.Zero;
            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(_yaw)) * (float)Math.Cos(MathHelper.DegreesToRadians(_pitch));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(_pitch));
            front.Z = (float)Math.Sin(MathHelper.DegreesToRadians(_yaw)) * (float)Math.Cos(MathHelper.DegreesToRadians(_pitch));
            _cameraFront = Vector3.Normalize(front);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.M:
                    _renderMode = _renderMode == RenderMode.Fullscreen2D ? RenderMode.Model3D : RenderMode.Fullscreen2D;
                    Console.WriteLine("Render mode: " + _renderMode);
                    break;
                case Keys.Q:
                    _currentShaderIndex = (_currentShaderIndex - 1 + _availableShaders.Count) % _availableShaders.Count;
                    _selectedShader = _availableShaders[_currentShaderIndex];
                    LoadShader(_selectedShader);
                    break;
                case Keys.E:
                    _currentShaderIndex = (_currentShaderIndex + 1) % _availableShaders.Count;
                    _selectedShader = _availableShaders[_currentShaderIndex];
                    LoadShader(_selectedShader);
                    break;
                case Keys.Z:
                    _currentModelIndex = (_currentModelIndex - 1 + _availableModels.Count) % _availableModels.Count;
                    _selectedModel = _availableModels[_currentModelIndex];
                    LoadModel(_selectedModel);
                    break;
                case Keys.X:
                    _currentModelIndex = (_currentModelIndex + 1) % _availableModels.Count;
                    _selectedModel = _availableModels[_currentModelIndex];
                    LoadModel(_selectedModel);
                    break;
                case Keys.R:
                    _cameraPos = new Vector3(0.0f, 0.0f, 3.0f);
                    _cameraFront = new Vector3(0.0f, 0.0f, -1.0f);
                    _cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
                    _yaw = -90.0f;
                    _pitch = 0.0f;
                    _fov = 45.0f;
                    _rotationY = 0.0f;
                    break;
                case Keys.B:
                    _useBuffers = !_useBuffers;
                    Console.WriteLine("Buffer system: " + (_useBuffers ? "ON" : "OFF"));
                    break;
                case Keys.H:
                    _showHelp = !_showHelp;
                    Console.WriteLine(_showHelp ? "Ajuda ativada" : "Ajuda desativada");
                    if (_showHelp)
                    {
                        Console.WriteLine($"Pasta de texturas: {_resolvedTextureDir}");
                        Console.WriteLine("Texturas carregadas:");
                        for (int i = 0; i < _loadedTextureNames.Count; i++)
                            Console.WriteLine($"  texture{i}: {_loadedTextureNames[i]}");
                    }
                    break;
                case Keys.F5:
                    Console.WriteLine("[TEXTURE] Recarregando...");
                    _resolvedTextureDir = ResolveTexturesFolder();
                    LoadTexturesFromFolder(_resolvedTextureDir, includeSubdirs: false);
                    break;
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            var input = KeyboardState;
            var speed = 2.5f * (float)args.Time;
            if (input.IsKeyDown(Keys.W)) _cameraPos += _cameraFront * speed;
            if (input.IsKeyDown(Keys.S)) _cameraPos -= _cameraFront * speed;
            if (input.IsKeyDown(Keys.A)) _cameraPos -= Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * speed;
            if (input.IsKeyDown(Keys.D)) _cameraPos += Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * speed;
            _rotationY += (float)args.Time * 30.0f;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            _time += (float)args.Time;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_shader != null)
            {
                _shader.Use();
                SetCommonUniforms(_shader);

                if (_renderMode == RenderMode.Fullscreen2D)
                {
                    if (_useBuffers)
                        RenderWithBuffers2D();
                    else
                        RenderDirect();
                }
                else
                {
                    if (_useBuffers)
                        RenderWithBuffers3D();
                    else
                    {
                        var model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotationY));
                        var view = Matrix4.LookAt(_cameraPos, _cameraPos + _cameraFront, _cameraUp);
                        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fov), Size.X / (float)Size.Y, 0.1f, 100.0f);
                        _shader.SetMatrix4("model", model);
                        _shader.SetMatrix4("view", view);
                        _shader.SetMatrix4("projection", projection);
                        _model?.Render();
                    }
                }
            }

            SwapBuffers();
        }

        private void RenderWithBuffers2D()
        {
            _bufferManager.BindCurrentBufferForWriting();

            _shader?.Use();
            SetCommonUniforms(_shader);

            // Bind previous frame como iChannel0
            _bufferManager.BindBuffersForReading(_shader);

            // Renderiza quad fullscreen
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            _bufferManager.UnbindBuffers();

            // Copia para tela com copy shader
            GL.Viewport(0, 0, Size.X, Size.Y);
            if (_copyShader != null)
            {
                _copyShader.Use();
                _copyShader.SetVector2("iResolution", new Vector2(Size.X, Size.Y));
                var currentBuffer = _bufferManager.GetCurrentBuffer();
                currentBuffer.BindForReading(TextureUnit.Texture0);
                _copyShader.SetTexture("inputTexture", 0);

                // Opcional: também disponibiliza texturas no copy pass
                GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out int maxUnits);
                int startUnit = 1;
                int canBind = Math.Min(_loadedTextures.Count, Math.Max(0, maxUnits - startUnit));
                for (int i = 0; i < canBind; i++)
                {
                    int unit = startUnit + i;
                    GL.ActiveTexture(TextureUnit.Texture0 + unit);
                    GL.BindTexture(TextureTarget.Texture2D, _loadedTextures[i]);
                    _copyShader.SetInt($"texture{i}", unit);
                }

                GL.BindVertexArray(_vao);
                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            }

            _bufferManager.SwapBuffers();
        }

        private void RenderWithBuffers3D()
        {
            _bufferManager.BindCurrentBufferForWriting();

            _shader?.Use();
            SetCommonUniforms(_shader);

            var model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotationY));
            var view = Matrix4.LookAt(_cameraPos, _cameraPos + _cameraFront, _cameraUp);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fov), Size.X / (float)Size.Y, 0.1f, 100.0f);

            _bufferManager.BindBuffersForReading(_shader);

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _model?.Render();

            _bufferManager.UnbindBuffers();

            GL.Viewport(0, 0, Size.X, Size.Y);
            if (_copyShader != null)
            {
                _copyShader.Use();
                _copyShader.SetVector2("iResolution", new Vector2(Size.X, Size.Y));
                var currentBuffer = _bufferManager.GetCurrentBuffer();
                currentBuffer.BindForReading(TextureUnit.Texture0);
                _copyShader.SetTexture("inputTexture", 0);

                // Opcional: também disponibiliza texturas no copy pass
                GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out int maxUnits);
                int startUnit = 1;
                int canBind = Math.Min(_loadedTextures.Count, Math.Max(0, maxUnits - startUnit));
                for (int i = 0; i < canBind; i++)
                {
                    int unit = startUnit + i;
                    GL.ActiveTexture(TextureUnit.Texture0 + unit);
                    GL.BindTexture(TextureTarget.Texture2D, _loadedTextures[i]);
                    _copyShader.SetInt($"texture{i}", unit);
                }

                GL.BindVertexArray(_vao);
                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            }

            _bufferManager.SwapBuffers();
        }

        private void RenderDirect()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        protected override void OnUnload()
        {
            _shader?.Dispose();
            _model?.Dispose();
            _copyShader?.Dispose();
            _bufferManager?.Dispose();

            // Libera as texturas carregadas
            ReleaseLoadedTextures();

            if (_vao != 0) GL.DeleteVertexArray(_vao);
            if (_vbo != 0) GL.DeleteBuffer(_vbo);
            if (_ebo != 0) GL.DeleteBuffer(_ebo);
            base.OnUnload();
        }
    }
}
