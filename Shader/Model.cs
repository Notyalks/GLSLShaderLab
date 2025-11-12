using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assimp;
using OpenTK.Mathematics;
using GLPrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace GLSLShaderLab
{
    public class Model : IDisposable
    {
        private List<Mesh> _meshes;
        private string _directory;
        private bool _disposed = false;

        public List<Mesh> Meshes => _meshes;
        public string Name { get; private set; }

        public Model(string path)
        {
            _meshes = new List<Mesh>();
            _directory = Path.GetDirectoryName(path) ?? "";
            Name = Path.GetFileNameWithoutExtension(path);
            LoadModel(path);
        }

        private void LoadModel(string path)
        {
            var importer = new AssimpContext();
            
            // Configure import settings for better GLB support
            importer.SetConfig(new Assimp.Configs.FBXPreservePivotsConfig(false));
            
            var scene = importer.ImportFile(path, 
                PostProcessSteps.Triangulate | 
                PostProcessSteps.FlipUVs | 
                PostProcessSteps.CalculateTangentSpace |
                PostProcessSteps.GenerateNormals);

            if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete) || scene.RootNode == null)
            {
                throw new Exception($"Erro ao carregar modelo: Falha na importação do arquivo {path}");
            }

            ProcessNode(scene.RootNode, scene);
        }

        private void ProcessNode(Node node, Scene scene)
        {
            // Process all meshes in the current node
            for (int i = 0; i < node.MeshCount; i++)
            {
                var mesh = scene.Meshes[node.MeshIndices[i]];
                _meshes.Add(ProcessMesh(mesh, scene));
            }

            // Process all child nodes
            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene);
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {
            var vertices = new List<Mesh.Vertex>();
            var indices = new List<uint>();

            // Process vertices
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var vertex = new Mesh.Vertex();

                // Position
                if (mesh.HasVertices)
                {
                    vertex.Position = new Vector3(
                        mesh.Vertices[i].X,
                        mesh.Vertices[i].Y,
                        mesh.Vertices[i].Z);
                }

                // Normal
                if (mesh.HasNormals)
                {
                    vertex.Normal = new Vector3(
                        mesh.Normals[i].X,
                        mesh.Normals[i].Y,
                        mesh.Normals[i].Z);
                }
                else
                {
                    vertex.Normal = Vector3.UnitY; // Default normal
                }

                // Texture coordinates
                if (mesh.HasTextureCoords(0))
                {
                    vertex.TexCoord = new Vector2(
                        mesh.TextureCoordinateChannels[0][i].X,
                        mesh.TextureCoordinateChannels[0][i].Y);
                }
                else
                {
                    vertex.TexCoord = Vector2.Zero;
                }

                vertices.Add(vertex);
            }

            // Process indices
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                var face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add((uint)face.Indices[j]);
                }
            }

            return new Mesh(vertices, indices);
        }

        public void Render()
        {
            foreach (var mesh in _meshes)
            {
                mesh.Render();
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
                    foreach (var mesh in _meshes)
                    {
                        mesh.Dispose();
                    }
                    _meshes.Clear();
                }
                _disposed = true;
            }
        }

        ~Model()
        {
            if (!_disposed)
            {
                Console.WriteLine("GPU Resource leak! Model was not disposed properly.");
            }
        }
    }
}