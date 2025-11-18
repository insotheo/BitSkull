using BitSkull.Core;
using BitSkull.Graphics;
using BitSkull.Graphics.Queue;
using Silk.NET.OpenGL;
using System.Collections.Generic;

namespace BitSkull.Platform.OpenGL
{
    internal sealed class OpenGLBackend : IRenderBackend
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
            Gl.ClearColor(r, g, b, a);
            Clear();
        }

        public void ResizeFramebuffer(int x, int y) => Gl.Viewport(0, 0, (uint)x, (uint)y);

        public IPlatformRenderable CreatePlatformRenderable(VertexBuffer vertexBuffer, IndexBuffer indexBuffer) => new OpenGLRenderable(vertexBuffer, indexBuffer);

        public unsafe void Draw(Graphics.Shader shader, List<Renderable> links)
        {
            GL gl = (Application.GetAppRenderer().Context as OpenGLBackend).Gl;

            shader.Use();

            foreach (Renderable link in links)
            {
                link.Material.Apply();
                link.Platform.Bind();
                gl.DrawElements(GLEnum.Triangles, link.IBuffer.GetCount(), GLEnum.UnsignedInt, null);
                link.Platform.Unbind();
            }

            shader.ZeroUse();
        }
    }
}
