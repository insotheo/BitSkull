using System;

namespace BitSkull.Graphics
{
    public class Shader : IDisposable
    {
        protected Shader() { }

        public virtual void Use() { }
        public virtual void ZeroUse() { }
        public virtual void Dispose() { }
    }
}
