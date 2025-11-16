using BitSkull.Graphics;
using Silk.NET.OpenGL;

namespace BitSkull.Platform.OpenGL
{
    internal sealed class GlRendererContext : IRendererContext
    {
        internal GL Gl { get; private set; }

        public void Initialize(Silk.NET.Core.Contexts.INativeContext ctx) => Gl = GL.GetApi(ctx);
        public void Dispose() => Gl.CurrentVTable.Dispose();

        public void Configure()
        {
            Gl.Enable(GLEnum.ColorBufferBit | GLEnum.DepthBufferBit);
        }

        public void Clear() => Gl.Clear(ClearBufferMask.ColorBufferBit);

        public void Clear(float r, float g, float b, float a)
        {
            Clear();
            Gl.ClearColor(r, g, b, a);
        }

        public void ResizeFramebuffer(int x, int y) => Gl.Viewport(0, 0, (uint)x, (uint)y);
    }
}
