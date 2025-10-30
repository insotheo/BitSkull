using BitSkull.Events;

namespace BitSkull.Core
{
    public class Layer
    {
        public string Name { get; }
        
        public Layer(string name)
        {
            Name = name;
        }

        public virtual void OnAttach() { }
        public virtual void OnDetach() { }
        public virtual void OnUpdate(float dt) { }
        public virtual void OnRender() { }
        public virtual void OnEvent(Event ev) { }
    }
}
