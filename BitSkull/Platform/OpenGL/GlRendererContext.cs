using BitSkull.Graphics;
using Silk.NET.OpenGL;

namespace BitSkull.Platform.OpenGL
{
    internal sealed class GlRendererContext : IRendererContext
    {
        private GL _gl;

        public void Initialize(Silk.NET.Core.Contexts.INativeContext ctx) => _gl = GL.GetApi(ctx);
        public void Dispose() => _gl.CurrentVTable.Dispose();

        public void Configure()
        {
            _gl.Enable(GLEnum.ColorBufferBit);
        }

        public void Clear() => _gl.Clear(ClearBufferMask.ColorBufferBit);

        public void Clear(float r, float g, float b, float a)
        {
            Clear();
            _gl.ClearColor(r, g, b, a);
        }

        public void ResizeFramebuffer(int x, int y) => _gl.Viewport(0, 0, (uint)x, (uint)y);
    }
}
