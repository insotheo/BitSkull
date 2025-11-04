using BitSkull.InputSystem;

namespace BitSkull.Events
{
    public abstract class MouseEvent : Event
    {
        public MouseEvent() : base(EventType.MouseEvent)
        { }
    }

    public sealed class MouseMovedEvent : MouseEvent
    {
        public float X { get; }
        public float Y { get; }

        public MouseMovedEvent(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"Mouse moved event({X}, {Y})";
    }

    public sealed class MouseScrollEvent : MouseEvent
    {
        public float XOffset { get; }
        public float YOffset { get; }

        public MouseScrollEvent(float xOffset, float yOffset)
        {
            XOffset = xOffset;
            YOffset = yOffset;
        }

        public override string ToString() => $"Mouse scrolled event({XOffset}, {YOffset})";
    }

    public sealed class MouseButtonPressed : MouseEvent
    {
        public MouseButton Button { get; }

        public MouseButtonPressed(MouseButton button)
        {
            Button = button;
        }

        public override string ToString() => $"Button pressed event({Button})";
    }

    public sealed class MouseButtonReleased : MouseEvent
    {
        public MouseButton Button { get; }

        public MouseButtonReleased(MouseButton button)
        {
            Button = button;
        }

        public override string ToString() => $"Button released event({Button})";
    }
}
