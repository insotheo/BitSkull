namespace BitSkull.Events
{
    public abstract class WindowEvent : Event
    {
        public WindowEvent() : base(EventType.WindowEvent)
        { }
    }

    public sealed class WindowResizeEvent : WindowEvent
    {
        public int Width { get; }
        public int Height { get; }

        public WindowResizeEvent(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString() => $"Window resize event({Width}, {Height})";
    }

    public sealed class WindowMoveEvent : WindowEvent
    {
        public int X { get; }
        public int Y { get; }

        public WindowMoveEvent(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString() => $"Window move event({X}, {Y})";
    }

    public sealed class WindowFocusEvent : WindowEvent
    {
        public bool IsFocused { get; }

        public WindowFocusEvent(bool isFocused)
        {
            IsFocused = isFocused;
        }

        public override string ToString() => $"Window {(IsFocused ? "got" : "lost")} focus";
    }
}
