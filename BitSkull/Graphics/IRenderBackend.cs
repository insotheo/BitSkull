using BitSkull.Graphics.Queue;
using System;
using System.Collections.Generic;

namespace BitSkull.Graphics
{
    public interface IRenderBackend : IDisposable
    {
        public void Initialize(Silk.NET.Core.Contexts.INativeContext ctx);
        public void Configure();

        public void Clear();
        public void Clear(float r, float g, float b, float a);
        public void ResizeFramebuffer(int x, int y);

        public IPlatformRenderable CreatePlatformRenderable(VertexBuffer vertexBuffer, IndexBuffer indexBuffer);

        void Draw(Shader shader, List<Renderable> renderables);

        VertexBuffer GenVertexBuffer(float[] vertices);
        IndexBuffer GenIndexBuffer(uint[] indices);
        Shader GenShader(string vertexShader, string fragmentShader);
    }
}
