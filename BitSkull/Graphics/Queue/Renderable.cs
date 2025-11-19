using BitSkull.Core;
using System;
using System.Collections.Generic;

namespace BitSkull.Graphics.Queue
{
    public class Renderable : IDisposable
    {
        internal VertexBuffer VBuffer { get; private set; }
        internal IndexBuffer IBuffer { get; private set; }
        internal Shader Shader { get; private set; }
        internal Material Material { get; private set; }


        internal IPlatformRenderable Platform { get; private set; } = null;
        internal bool HasPlatform => Platform != null;

        internal Renderable(VertexBuffer vbo, IndexBuffer ibo, Shader shader)
        {
            VBuffer = vbo;
            IBuffer = ibo;
            Shader = shader;
            Material = new Material(shader);
        }

        internal void Bake()
        {
            DetachPlatform();

            List<uint> counts = new();
            IPlatformRenderable platform = Application.GetAppRenderer().Backend.CreatePlatformRenderable(VBuffer, IBuffer);
            Platform = platform;
        }

        internal void DetachPlatform()
        {
            Platform?.Dispose();
            Platform = null;
        }

        public void Dispose(bool disposeShader = false)
        {
            DetachPlatform();

            if (disposeShader)
                Shader.Dispose();

            Material.ClearPending();
            Material = null;

            VBuffer.Dispose();
            VBuffer = null;

            IBuffer.Dispose();
            IBuffer = null;
        }

        public void Dispose() => Dispose(false);
    }
}
