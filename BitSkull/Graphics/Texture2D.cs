using System;

namespace BitSkull.Graphics
{
    public class Texture2D : IDisposable
    {
        protected Texture2D() { }

        public virtual void Bind() { }
        public virtual void Unbind() { }

        public virtual void Dispose() { }
    }
}
