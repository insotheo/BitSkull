using System.Collections.Generic;

namespace BitSkull.Graphics.Queue
{
    internal class RenderQueue
    {
        internal Dictionary<Shader, List<Renderable>> Queue { get; private set; }
        internal bool Initialized => Queue != null;

        internal RenderQueue()
        {
            Queue = new Dictionary<Shader, List<Renderable>>();
        }

        internal void Push(Renderable item)
        {
            if (!Queue.ContainsKey(item.Shader))
                Queue[item.Shader] = new List<Renderable>();
            Queue[item.Shader].Add(item);
        }

        internal void Bake()
        {
            foreach (List<Renderable> group in Queue.Values)
                foreach (Renderable item in group)
                    item.Bake();
        }

        internal void Dispose()
        {
            if (Queue == null)
                return;

            foreach (var group in Queue.Values)
            {
                foreach (var item in group)
                    item.Dispose();
                group.Clear();
            }
            foreach (Shader shader in Queue.Keys)
                shader.Dispose();

            Queue.Clear();
        }
    }
}
