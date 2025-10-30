namespace BitSkull.Events
{
    public abstract class KeyboardEvent : Event
    {
        public KeyboardEvent() : base(EventType.KeyboardEvent)
        { }
    }

    public sealed class KeyPressedEvent : KeyboardEvent
    {
        public int KeyCode { get; }
        public bool IsRepeat { get; }

        public KeyPressedEvent(int key, bool isRepeat = false)
        {
            KeyCode = key;
            IsRepeat = isRepeat;
        }

        public override string ToString() => $"Key pressed event({KeyCode})";
    }

    public sealed class KeyReleasedEvent : KeyboardEvent
    {
        public int KeyCode { get; }

        public KeyReleasedEvent(int key)
        {
            KeyCode = key;
        }
        public override string ToString() => $"Key released event({KeyCode})";
    }

    public sealed class KeyTypedEvent : KeyboardEvent
    {
        public char Character { get; }

        public KeyTypedEvent(char character)
        {
            Character = character;
        }
        public override string ToString() => $"Key typed event({Character})";
    }
}
