using BitSkull.Core;
using BitSkull.Graphics;
using BitSkull.Graphics.Queue;
using Silk.NET.OpenGL;

namespace BitSkull.Platform.OpenGL
{
    internal sealed class OpenGLRenderable : IPlatformRenderable
    {
        private uint _vao;

        internal OpenGLRenderable(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            if (vertexBuffer == null || indexBuffer == null)
                return;

            GL gl = (Application.GetAppRenderer().Context as OpenGLBackend).Gl;

            _vao = gl.GenVertexArray();
            gl.BindVertexArray(_vao);

            vertexBuffer.Bind();
            vertexBuffer.BindLayout();

            indexBuffer.Bind();

            gl.BindVertexArray(0);
            vertexBuffer.Unbind();
            indexBuffer.Unbind();
        }

        public void Dispose() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.DeleteVertexArray(_vao);


        public void Unbind() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.BindVertexArray(0);
        public void Bind() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.BindVertexArray(_vao);
    }
}
