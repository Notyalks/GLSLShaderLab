using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GLSLShaderLab
{
    public class ShaderSelector
    {
        public class ShaderInfo
        {
            public string Name { get; set; }
            public string VertexPath { get; set; }
            public string FragmentPath { get; set; }
            
            public ShaderInfo(string name, string vertexPath, string fragmentPath)
            {
                Name = name;
                VertexPath = vertexPath;
                FragmentPath = fragmentPath;
            }
            
            public override string ToString()
            {
                return $"{Name} (Vertex: {Path.GetFileName(VertexPath)}, Fragment: {Path.GetFileName(FragmentPath)})";
            }
        }

        private List<ShaderInfo> _availableShaders;
        private const string ShadersPath = "Shaders";

        public ShaderSelector()
        {
            _availableShaders = ScanForShaders();
        }

        private List<ShaderInfo> ScanForShaders()
        {
            var shaders = new List<ShaderInfo>();
            
            if (!Directory.Exists(ShadersPath))
            {
                Console.WriteLine($"Diretório {ShadersPath} não encontrado!");
                return shaders;
            }

            // Buscar por pares .vert/.frag
            var vertFiles = Directory.GetFiles(ShadersPath, "*.vert", SearchOption.AllDirectories);
            
            foreach (var vertFile in vertFiles)
            {
                var baseName = Path.GetFileNameWithoutExtension(vertFile);
                var directory = Path.GetDirectoryName(vertFile);
                var fragFile = Path.Combine(directory!, $"{baseName}.frag");
                
                if (File.Exists(fragFile))
                {
                    var displayName = Path.GetRelativePath(ShadersPath, directory!) == "." 
                        ? baseName 
                        : $"{Path.GetRelativePath(ShadersPath, directory!)}/{baseName}";
                        
                    shaders.Add(new ShaderInfo(displayName, vertFile, fragFile));
                }
                else
                {
                    Console.WriteLine($"Aviso: Arquivo .frag correspondente não encontrado para {vertFile}");
                }
            }

            // Ordenar por nome para consistência
            shaders.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            
            return shaders;
        }

        public ShaderInfo? SelectShader()
        {
            if (_availableShaders.Count == 0)
            {
                Console.WriteLine("Nenhum shader encontrado no diretório Shaders!");
                Console.WriteLine("Certifique-se de que existem pares de arquivos .vert e .frag no diretório Shaders.");
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para continuar...");
                Console.ReadKey();
                return null;
            }

            Console.Clear();
            Console.WriteLine("=== SELETOR DE SHADERS ===");
            Console.WriteLine();
            Console.WriteLine($"Diretório de shaders: {ShadersPath}");
            Console.WriteLine($"{_availableShaders.Count} shader(s) encontrado(s)");
            Console.WriteLine();
            Console.WriteLine("Shaders disponíveis:");
            Console.WriteLine("".PadRight(50, '─'));

            for (int i = 0; i < _availableShaders.Count; i++)
            {
                Console.WriteLine($"  {i + 1,2}. {_availableShaders[i].Name}");
            }

            Console.WriteLine("".PadRight(50, '─'));
            Console.WriteLine();
            Console.Write("Selecione um shader (número) ou 'r' para recarregar: ");

            var input = Console.ReadLine()?.Trim().ToLower();
            
            if (input == "r")
            {
                Console.WriteLine("Recarregando lista de shaders...");
                _availableShaders = ScanForShaders();
                return SelectShader(); // Recursivo para mostrar a nova lista
            }

            if (int.TryParse(input, out int selection) && 
                selection >= 1 && selection <= _availableShaders.Count)
            {
                var selectedShader = _availableShaders[selection - 1];
                Console.WriteLine();
                Console.WriteLine("Shader selecionado:");
                Console.WriteLine($"   Nome: {selectedShader.Name}");
                Console.WriteLine($"   Vertex: {selectedShader.VertexPath}");
                Console.WriteLine($"   Fragment: {selectedShader.FragmentPath}");
                Console.WriteLine();
                Console.WriteLine("Iniciando aplicação...");
                Console.WriteLine();
                return selectedShader;
            }
            else
            {
                Console.WriteLine("Seleção inválida!");
                Console.WriteLine("Pressione qualquer tecla para tentar novamente...");
                Console.ReadKey();
                return SelectShader(); // Recursivo para tentar novamente
            }
        }
        
        public List<ShaderInfo> GetAvailableShaders() => _availableShaders;
    }
}