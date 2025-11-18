using BitSkull.Graphics;
using BitSkull.Graphics.Chain;
using Silk.NET.OpenGL;
using System.Collections.Generic;

namespace BitSkull.Platform.OpenGL
{
    internal sealed class GlRendererContext : IRendererContext
    {
        internal GL Gl { get; private set; }

        public void Initialize(Silk.NET.Core.Contexts.INativeContext ctx) => Gl = GL.GetApi(ctx);
        public void Dispose() => Gl.CurrentVTable.Dispose();

        public void Configure()
        {
            //Gl.Enable(GLEnum.DepthTest);
            //Gl.Enable(GLEnum.CullFace);
            //Gl.DepthFunc(GLEnum.Less);
            Gl.Enable(GLEnum.Blend);
            Gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        }

        public void Clear() => Gl.Clear(ClearBufferMask.ColorBufferBit);

        public void Clear(float r, float g, float b, float a)
        {
            Clear();
            Gl.ClearColor(r, g, b, a);
        }

        public void ResizeFramebuffer(int x, int y) => Gl.Viewport(0, 0, (uint)x, (uint)y);

        public IPlatformChainLink GenPlatformChainLink(VertexBuffer vertexBuffer, IndexBuffer indexBuffer) => new OpenGLChainLink(vertexBuffer, indexBuffer);

        public unsafe void Draw(Graphics.Shader shader, List<ChainLink> links)
        {
            GL gl = (Renderer.Context as GlRendererContext).Gl;

            shader.Use();

            foreach (ChainLink link in links)
            {
                link.Material.Apply();
                link.Platform.Use();
                gl.DrawElements(GLEnum.Triangles, link.IBuffer.GetCount(), GLEnum.UnsignedInt, null);
                link.Platform.Unuse();
            }

            shader.ZeroUse();
        }
    }
}
