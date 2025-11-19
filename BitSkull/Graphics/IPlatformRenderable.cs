using System;

namespace BitSkull.Graphics
{
    public interface IPlatformRenderable : IDisposable
    {
        public void Bind();
        public void Unbind();
    }
}
