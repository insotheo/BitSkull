using System.Collections.Generic;

namespace BitSkull.Graphics.Queue
{
    internal static class RenderQueue
    {
        internal static Dictionary<Shader, List<Renderable>> Queue { get; private set; }
        internal static bool Initialized => Queue != null;

        public static void Initialize()
        {
            Queue = new Dictionary<Shader, List<Renderable>>();
        }

        internal static void Push(Renderable item)
        {
            if (!Queue.ContainsKey(item.Shader))
                Queue[item.Shader] = new List<Renderable>();
            Queue[item.Shader].Add(item);
        }

        internal static void Bake()
        {
            foreach (List<Renderable> group in Queue.Values)
                foreach (Renderable item in group)
                    item.Bake();
        }

        internal static void Dispose()
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
