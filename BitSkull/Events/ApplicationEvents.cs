namespace BitSkull.Events
{
    public abstract class ApplicationEvent : Event
    {
        public ApplicationEvent() : base(EventType.ApplicationEvent)
        { }
    }

    public sealed class AppTickEvent() : ApplicationEvent { }
    public sealed class AppUpdateEvent() : ApplicationEvent { }
    public sealed class AppRenderEvent() : ApplicationEvent { }
    public sealed class AppCloseEvent() : ApplicationEvent { }
}
