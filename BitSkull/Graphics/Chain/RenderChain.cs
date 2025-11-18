using System.Collections.Generic;

namespace BitSkull.Graphics.Chain
{
    internal static class RenderChain
    {
        internal static Dictionary<Shader, List<ChainLink>> GroupedLinks { get; private set; }
        internal static bool Initialized => GroupedLinks != null;

        public static void Initialize()
        {
            GroupedLinks = new Dictionary<Shader, List<ChainLink>>();
        }

        internal static void PushLink(ChainLink link)
        {
            if(!GroupedLinks.ContainsKey(link.Shader))
                GroupedLinks[link.Shader] = new List<ChainLink>();
            GroupedLinks[link.Shader].Add(link);
        }

        internal static void Bake()
        {
            foreach (List<ChainLink> group in GroupedLinks.Values)
                foreach(ChainLink link in group)
                    link.Bake();
        }

        internal static void Dispose()
        {
            if (GroupedLinks == null)
                return;

            foreach (var group in GroupedLinks.Values)
            {
                foreach (var link in group)
                    link.Dispose();
                group.Clear();
            }
            foreach (Shader shader in GroupedLinks.Keys)
                shader.Dispose();
            GroupedLinks.Clear();
        }
    }
}
