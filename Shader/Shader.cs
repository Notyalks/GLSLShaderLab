using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GLSLShaderLab
{
    public class Shader : IDisposable
    {
        public int Handle;
        private bool _disposed = false;

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexCode = File.ReadAllText(vertexPath);
            string fragmentCode = File.ReadAllText(fragmentPath);

            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexCode);
            GL.CompileShader(vertex);
            
            string vertexLog = GL.GetShaderInfoLog(vertex);
            if (!string.IsNullOrEmpty(vertexLog))
                Console.WriteLine($"Vertex Shader Log: {vertexLog}");

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentCode);
            GL.CompileShader(fragment);
            
            string fragmentLog = GL.GetShaderInfoLog(fragment);
            if (!string.IsNullOrEmpty(fragmentLog))
                Console.WriteLine($"Fragment Shader Log: {fragmentLog}");

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertex);
            GL.AttachShader(Handle, fragment);
            GL.LinkProgram(Handle);
            
            string programLog = GL.GetProgramInfoLog(Handle);
            if (!string.IsNullOrEmpty(programLog))
                Console.WriteLine($"Program Log: {programLog}");

            GL.DetachShader(Handle, vertex);
            GL.DetachShader(Handle, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
        }

        public void Use() => GL.UseProgram(Handle);

        public void SetFloat(string name, float value) =>
            GL.Uniform1(GL.GetUniformLocation(Handle, name), value);

        public void SetVector2(string name, Vector2 value) =>
            GL.Uniform2(GL.GetUniformLocation(Handle, name), value);


        public void SetVector3(string name, Vector3 value) =>
            GL.Uniform3(GL.GetUniformLocation(Handle, name), value);

        public void SetMatrix4(string name, Matrix4 value) =>
            GL.UniformMatrix4(GL.GetUniformLocation(Handle, name), false, ref value);

        public void SetInt(string name, int value) =>
            GL.Uniform1(GL.GetUniformLocation(Handle, name), value);

        public void SetTexture(string name, int textureUnit)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location >= 0)
            {
                GL.Uniform1(location, textureUnit);
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    GL.DeleteProgram(Handle);
                }
                _disposed = true;
            }
        }

        ~Shader()
        {
            if (!_disposed)
            {
                Console.WriteLine("GPU Resource leak! Shader was not disposed properly.");
            }
        }
    }
}
