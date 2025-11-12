using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GLSLShaderLab
{
    public class RenderBuffer : IDisposable
    {
        public int TextureId { get; private set; }
        public int FramebufferId { get; private set; }
        public Vector2i Size { get; private set; }
        private bool _disposed = false;

        public RenderBuffer(int width, int height)
        {
            Size = new Vector2i(width, height);
            CreateBuffer();
        }

        private void CreateBuffer()
        {
            // Create texture
            TextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Size.X, Size.Y, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Create framebuffer
            FramebufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, TextureId, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Framebuffer is not complete!");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Resize(int width, int height)
        {
            if (Size.X == width && Size.Y == height) return;

            Size = new Vector2i(width, height);
            
            // Recreate texture with new size
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Size.X, Size.Y, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        }

        public void BindForWriting()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferId);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        public void BindForReading(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
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
                    if (TextureId != 0)
                    {
                        GL.DeleteTexture(TextureId);
                        TextureId = 0;
                    }
                    
                    if (FramebufferId != 0)
                    {
                        GL.DeleteFramebuffer(FramebufferId);
                        FramebufferId = 0;
                    }
                }
                _disposed = true;
            }
        }

        ~RenderBuffer()
        {
            if (!_disposed)
            {
                Console.WriteLine("GPU Resource leak! RenderBuffer was not disposed properly.");
            }
        }
    }
}