using System;

namespace BitSkull.Graphics.Queue
{
    public interface IPlatformRenderable : IDisposable
    {
        public void Bind();
        public void Unbind();
    }
}
