using System;

namespace BitSkull.Graphics
{
    public sealed class Mesh : IDisposable
    {
        private static int _nextID = 0;
        public int ID { get; private set; }

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private IPlatformRenderable _platformRenderable;

        internal Mesh(VertexBuffer vbo, IndexBuffer ibo, Renderer renderer)
        {
            ID = _nextID++;
            _vertexBuffer = vbo;
            _indexBuffer = ibo;
            _platformRenderable = renderer.Backend.CreatePlatformRenderable(vbo, ibo);
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _platformRenderable.Dispose();
        }

        public void UsePlatform() => _platformRenderable.Bind();
        public uint GetIndexCount() => _indexBuffer.GetCount();
    }
}
