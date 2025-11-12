using System;
using System.Collections.Generic;
using System.Linq;
using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using GLPrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace GLSLShaderLab
{
    public class Mesh : IDisposable
    {
        public struct Vertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TexCoord;
            
            public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
            {
                Position = position;
                Normal = normal;
                TexCoord = texCoord;
            }
        }

        private List<Vertex> _vertices;
        private List<uint> _indices;
        private int _vao;
        private int _vbo;
        private int _ebo;
        private bool _disposed = false;

        public List<Vertex> Vertices => _vertices;
        public List<uint> Indices => _indices;
        public int VAO => _vao;

        public Mesh(List<Vertex> vertices, List<uint> indices)
        {
            _vertices = vertices;
            _indices = indices;
            SetupMesh();
        }

        private void SetupMesh()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            // Vertex buffer
            var vertexData = new float[_vertices.Count * 8]; // 3 pos + 3 normal + 2 texcoord
            for (int i = 0; i < _vertices.Count; i++)
            {
                var vertex = _vertices[i];
                var offset = i * 8;
                
                vertexData[offset + 0] = vertex.Position.X;
                vertexData[offset + 1] = vertex.Position.Y;
                vertexData[offset + 2] = vertex.Position.Z;
                
                vertexData[offset + 3] = vertex.Normal.X;
                vertexData[offset + 4] = vertex.Normal.Y;
                vertexData[offset + 5] = vertex.Normal.Z;
                
                vertexData[offset + 6] = vertex.TexCoord.X;
                vertexData[offset + 7] = vertex.TexCoord.Y;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

            // Element buffer
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);

            // Position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Normal attribute
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // Texture coordinate attribute
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(GLPrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
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
                    GL.DeleteVertexArray(_vao);
                    GL.DeleteBuffer(_vbo);
                    GL.DeleteBuffer(_ebo);
                }
                _disposed = true;
            }
        }

        ~Mesh()
        {
            if (!_disposed)
            {
                Console.WriteLine("GPU Resource leak! Mesh was not disposed properly.");
            }
        }
    }
}