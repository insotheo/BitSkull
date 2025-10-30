namespace BitSkull.Core
{
    public abstract class BaseWindow
    {
        public string Title { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public bool VSync { get; protected set; }


        internal virtual void Run() { }
        internal virtual void Close() { }
        internal virtual void SetVSync(bool vsync) { }
    }
}
