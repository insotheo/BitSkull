using System;

namespace BitSkull.Core
{
    public abstract class BaseWindow : IDisposable
    {
        public string Title { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public bool IsFocused { get; protected set; }
        public bool VSync { get; protected set; }


        internal virtual void Run() { }
        internal virtual void Close() { }
        internal virtual void SetVSync(bool vsync) { }

        internal virtual void DoUpdate(float dt) { }

        public virtual void Dispose() { }
    }
}
