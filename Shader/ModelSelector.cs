using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GLSLShaderLab
{
    public class ModelSelector
    {
        public class ModelInfo
        {
            public string Name { get; set; }
            public string FilePath { get; set; }
            
            public ModelInfo(string name, string filePath)
            {
                Name = name;
                FilePath = filePath;
            }
            
            public override string ToString()
            {
                return $"{Name} ({Path.GetFileName(FilePath)})";
            }
        }

        private List<ModelInfo> _availableModels;
        private const string ModelsPath = "Mesh";

        public ModelSelector()
        {
            _availableModels = ScanForModels();
        }

        private List<ModelInfo> ScanForModels()
        {
            var models = new List<ModelInfo>();
            
            if (!Directory.Exists(ModelsPath))
            {
                Console.WriteLine($"Diretório {ModelsPath} não encontrado!");
                return models;
            }

            // Supported 3D model formats
            var supportedExtensions = new[] { "*.glb", "*.gltf", "*.obj", "*.fbx", "*.dae", "*.3ds" };
            
            foreach (var extension in supportedExtensions)
            {
                var modelFiles = Directory.GetFiles(ModelsPath, extension, SearchOption.AllDirectories);
                
                foreach (var modelFile in modelFiles)
                {
                    var baseName = Path.GetFileNameWithoutExtension(modelFile);
                    var relativePath = Path.GetRelativePath(ModelsPath, Path.GetDirectoryName(modelFile)!);
                    
                    var displayName = relativePath == "." 
                        ? baseName 
                        : $"{relativePath}/{baseName}";
                        
                    models.Add(new ModelInfo(displayName, modelFile));
                }
            }

            // Sort by name for consistency
            models.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            
            return models;
        }

        public ModelInfo? SelectModel()
        {
            if (_availableModels.Count == 0)
            {
                Console.WriteLine("Nenhum modelo 3D encontrado no diretório Mesh!");
                Console.WriteLine("Suporte para: GLB, GLTF, OBJ, FBX, DAE, 3DS");
                Console.WriteLine("Certifique-se de que existem arquivos de modelo no diretório Mesh.");
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para continuar...");
                Console.ReadKey();
                return null;
            }

            Console.Clear();
            Console.WriteLine("=== SELETOR DE MODELOS 3D ===");
            Console.WriteLine();
            Console.WriteLine($"Diretório de modelos: {ModelsPath}");
            Console.WriteLine($"{_availableModels.Count} modelo(s) encontrado(s)");
            Console.WriteLine();
            Console.WriteLine("Modelos disponíveis:");
            Console.WriteLine("".PadRight(50, '?'));

            for (int i = 0; i < _availableModels.Count; i++)
            {
                Console.WriteLine($"  {i + 1,2}. {_availableModels[i].Name}");
            }

            Console.WriteLine("".PadRight(50, '?'));
            Console.WriteLine();
            Console.Write("Selecione um modelo (número) ou 'r' para recarregar: ");

            var input = Console.ReadLine()?.Trim().ToLower();
            
            if (input == "r")
            {
                Console.WriteLine("Recarregando lista de modelos...");
                _availableModels = ScanForModels();
                return SelectModel(); // Recursive to show the new list
            }

            if (int.TryParse(input, out int selection) && 
                selection >= 1 && selection <= _availableModels.Count)
            {
                var selectedModel = _availableModels[selection - 1];
                Console.WriteLine();
                Console.WriteLine("Modelo selecionado:");
                Console.WriteLine($"   Nome: {selectedModel.Name}");
                Console.WriteLine($"   Arquivo: {selectedModel.FilePath}");
                Console.WriteLine();
                return selectedModel;
            }
            else
            {
                Console.WriteLine("Seleção inválida!");
                Console.WriteLine("Pressione qualquer tecla para tentar novamente...");
                Console.ReadKey();
                return SelectModel(); // Recursive to try again
            }
        }
        
        public List<ModelInfo> GetAvailableModels() => _availableModels;
    }
}