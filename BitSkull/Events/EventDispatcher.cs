using System;

namespace BitSkull.Events
{
    public sealed class EventDispatcher
    {
        private Event _event;
        
        public EventDispatcher(Event e)
        {
            _event = e;
        }

        public bool Dispatch<T>(Func<T, bool> func) where T : Event
        {
            if (_event.Handled) return false;

            if(_event is T ev)
            {
                if (func != null)
                {
                    _event.Handled = func.Invoke((T)_event);
                }
                return true;
            }
            return false;
        }
    }
}
