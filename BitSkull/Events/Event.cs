namespace BitSkull.Events
{
    public enum EventType
    {
        None = 0,
        ApplicationEvent = 1 << 2,
        WindowEvent = 1 << 3,
        KeyboardEvent = 1 << 4,
        MouseEvent = 1 << 5,
    }

    public abstract class Event
    {
        public readonly EventType Type;
        public bool Handled { get; internal set; }

        public Event(EventType type)
        {
            Type = type;
            Handled = false;
        }
    }
}
