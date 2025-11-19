using System.Collections.Generic;

namespace BitSkull.Graphics.Queue
{
    class RenderQueue
    {
        private Dictionary<Shader, List<Renderable>> _queue;
        internal bool Initialized => _queue != null;

        internal RenderQueue()
        {
            _queue = new Dictionary<Shader, List<Renderable>>();
        }

        internal void Push(Renderable item)
        {
            if (!_queue.ContainsKey(item.Shader))
                _queue[item.Shader] = new List<Renderable>();
            _queue[item.Shader].Add(item);
        }

        internal void Bake()
        {
            foreach (List<Renderable> group in _queue.Values)
                foreach (Renderable item in group)
                    item.Bake();
        }

        internal void Pop(Renderable item)
        {
            foreach(Shader shader in _queue.Keys)
            {
                if (_queue[shader].Contains(item))
                {
                    item.Dispose();
                    _queue[shader].Remove(item);
                    break;
                }
            }
        }

        internal Dictionary<Shader, List<Renderable>> GetQueue() => _queue;

        internal void Dispose()
        {
            if (_queue == null)
                return;

            foreach (var group in _queue.Values)
            {
                foreach (var item in group)
                    item.Dispose();
                group.Clear();
            }
            foreach (Shader shader in _queue.Keys)
                shader.Dispose();

            _queue.Clear();
        }
    }
}
