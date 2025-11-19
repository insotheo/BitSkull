using BitSkull.Graphics;
using BitSkull.Graphics.Queue;
using Silk.NET.OpenGL;

namespace BitSkull.Platform.OpenGL
{
    internal sealed class OpenGLRenderable : IPlatformRenderable
    {
        private uint _vao;

        private readonly GL _gl;

        internal OpenGLRenderable(GL gl, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            if (vertexBuffer == null || indexBuffer == null)
                return;

            _gl = gl;

            _vao = _gl.GenVertexArray();
            _gl.BindVertexArray(_vao);

            vertexBuffer.Bind();
            vertexBuffer.BindLayout();

            indexBuffer.Bind();

            _gl.BindVertexArray(0);
            vertexBuffer.Unbind();
            indexBuffer.Unbind();
        }

        public void Dispose() => _gl.DeleteVertexArray(_vao);


        public void Unbind() => _gl.BindVertexArray(0);
        public void Bind() => _gl.BindVertexArray(_vao);
    }
}
