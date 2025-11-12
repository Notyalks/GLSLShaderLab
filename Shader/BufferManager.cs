using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GLSLShaderLab
{
    public class BufferManager : IDisposable
    {
        private const int NUM_BUFFERS = 2; // Ping-pong buffers
        private List<RenderBuffer> _buffers;
        private int _currentBuffer = 0;
        private int _previousBuffer = 1;
        private bool _disposed = false;

        public BufferManager(int width, int height)
        {
            _buffers = new List<RenderBuffer>();
            for (int i = 0; i < NUM_BUFFERS; i++)
            {
                _buffers.Add(new RenderBuffer(width, height));
            }
        }

        public void Resize(int width, int height)
        {
            foreach (var buffer in _buffers)
            {
                buffer.Resize(width, height);
            }
        }

        public void SwapBuffers()
        {
            // Ping-pong between 0 and 1
            _previousBuffer = _currentBuffer;
            _currentBuffer = 1 - _currentBuffer;
        }

        public RenderBuffer GetCurrentBuffer()
        {
            return _buffers[_currentBuffer];
        }

        public RenderBuffer GetPreviousBuffer()
        {
            return _buffers[_previousBuffer];
        }

        public RenderBuffer GetBuffer(int index)
        {
            if (index >= 0 && index < NUM_BUFFERS)
                return _buffers[index];
            return null;
        }

        public void BindBuffersForReading(Shader shader)
        {
            // Bind previous buffer as iChannel0 (feedback)
            _buffers[_previousBuffer].BindForReading(TextureUnit.Texture0);
            shader.SetTexture("iChannel0", 0);
            
            // Bind other channels with the same buffer (for compatibility)
            for (int i = 1; i < 4; i++)
            {
                _buffers[_previousBuffer].BindForReading(TextureUnit.Texture0 + i);
                shader.SetTexture($"iChannel{i}", i);
            }
        }

        public void BindCurrentBufferForWriting()
        {
            GetCurrentBuffer().BindForWriting();
        }

        public void UnbindBuffers()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void ClearCurrentBuffer()
        {
            BindCurrentBufferForWriting();
            GL.Clear(ClearBufferMask.ColorBufferBit);
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
                    foreach (var buffer in _buffers)
                    {
                        buffer?.Dispose();
                    }
                    _buffers.Clear();
                }
                _disposed = true;
            }
        }

        ~BufferManager()
        {
            if (!_disposed)
            {
                Console.WriteLine("GPU Resource leak! BufferManager was not disposed properly.");
            }
        }
    }
}