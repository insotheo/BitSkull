using System;

namespace BitSkull.Graphics
{
    public interface IRendererContext : IDisposable
    {
        public void Initialize(Silk.NET.Core.Contexts.INativeContext ctx);
        public void Configure();

        public void Clear();
        public void Clear(float r, float g, float b, float a);
        public void ResizeFramebuffer(int x, int y);
    }
}
