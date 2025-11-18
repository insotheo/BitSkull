using System;

namespace BitSkull.Graphics
{
    public class IndexBuffer : IBuffer, IDisposable
    {
        protected IndexBuffer() { }

        public virtual void Bind() { }
        public virtual void Unbind() { }
        public virtual uint GetCount() { return 0; }
        public virtual void Dispose() { }
    }

    public class VertexBuffer : IBuffer, IDisposable
    {
        protected VertexBuffer() { }

        public virtual void Bind() { }
        public virtual void Unbind() { }
        public virtual void SetLayout(BufferLayout layot) { }
        public virtual BufferLayout GetLayot() { return null; }
        public virtual void BindLayout() { }
        public virtual void Dispose() { }
    }
}
