using BitSkull.InputSystem;

namespace BitSkull.Events
{
    public abstract class KeyboardEvent : Event
    {
        public KeyboardEvent() : base(EventType.KeyboardEvent)
        { }
    }

    public sealed class KeyPressedEvent : KeyboardEvent
    {
        public KeyCode KeyCode { get; }
        public bool IsRepeat { get; }

        public KeyPressedEvent(KeyCode key, bool isRepeat = false)
        {
            KeyCode = key;
            IsRepeat = isRepeat;
        }

        public override string ToString() => $"Key pressed event({KeyCode})";
    }

    public sealed class KeyReleasedEvent : KeyboardEvent
    {
        public KeyCode KeyCode { get; }

        public KeyReleasedEvent(KeyCode key)
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
