using System;

namespace BitSkull.Graphics.Chain
{
    public interface IPlatformChainLink : IDisposable
    {
        public void Use();
        public void Unuse();
    }
}
