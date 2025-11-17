using BitSkull.Graphics;
using BitSkull.Graphics.Chain;
using Silk.NET.OpenGL;

namespace BitSkull.Platform.OpenGL
{
    internal sealed class OpenGLChainLink : IPlatformChainLink
    {
        private uint _vao;

        internal OpenGLChainLink(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            if (vertexBuffer == null || indexBuffer == null)
                return;

            GL gl = (Renderer.Context as GlRendererContext).Gl;

            _vao = gl.GenVertexArray();
            gl.BindVertexArray(_vao);

            vertexBuffer.Bind();
            vertexBuffer.BindLayout();

            indexBuffer.Bind();

            gl.BindVertexArray(0);
            vertexBuffer.Unbind();
            indexBuffer.Unbind();
        }

        public void Dispose() => (Renderer.Context as GlRendererContext).Gl.DeleteVertexArray(_vao);


        public void Unuse() => (Renderer.Context as GlRendererContext).Gl.BindVertexArray(0);
        public void Use() => (Renderer.Context as GlRendererContext).Gl.BindVertexArray(_vao);
    }
}
