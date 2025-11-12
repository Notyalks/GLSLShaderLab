using System;

namespace GLSLShaderLab
{
    class Program
    {
        static void Main(string[] args)
        {
            // Select model first
            var modelSelector = new ModelSelector();
            var selectedModel = modelSelector.SelectModel();
            
            if (selectedModel == null)
            {
                Console.WriteLine("Nenhum modelo selecionado. Encerrando programa.");
                return;
            }
            
            // Then select shader
            var shaderSelector = new ShaderSelector();
            var selectedShader = shaderSelector.SelectShader();
            
            if (selectedShader != null)
            {
                using (var window = new Window(800, 600, "GLSL Shader Lab", selectedShader, selectedModel))
                {
                    window.Run();
                }
            }
            else
            {
                Console.WriteLine("Nenhum shader selecionado. Encerrando programa.");
            }
        }
    }
}
