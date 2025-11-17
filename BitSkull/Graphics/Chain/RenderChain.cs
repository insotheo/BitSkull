using System.Collections.Generic;

namespace BitSkull.Graphics.Chain
{
    internal static class RenderChain
    {
        private static List<ChainLink> _links;

        public static void Initialize()
        {
            _links = new List<ChainLink>();
        }

        internal static void PushLink(ChainLink link) => _links.Add(link);
        internal static void ClearChain()
        {
            foreach (ChainLink link in _links)
                link.Dispose();
            _links.Clear();
        }
        internal static List<ChainLink> GetChainLinks() => _links;

        internal static void Compress()
        {
            if(_links == null || _links.Count <= 1) return;

            Dictionary<Shader, List<ChainLink>> grouped = new();

            foreach(ChainLink link in _links)
            {
                if (link.Shader == null)
                    continue;

                if (!grouped.ContainsKey(link.Shader))
                    grouped[link.Shader] = new();

                grouped[link.Shader].Add(link);
            }

            List<ChainLink> compressed = new();

            foreach(var kvp in grouped)
            {
                var shader = kvp.Key;
                var links = kvp.Value;

                if(links.Count == 1)
                {
                    compressed.Add(links[0]);
                    continue;
                }

                ChainLink mergedLink = new(links[0].VBuffers[0], links[0].IBuffers[0], shader);
                for(int i = 1; i < links.Count; i++)
                {
                    mergedLink.AttachChainLink(links[i]);
                }

                compressed.Add(mergedLink);
            }

            _links = compressed;
        }


        internal static void Bake()
        {
            foreach (ChainLink link in _links)
                link.Bake();
        }

        internal static void Rebake()
        {
            foreach (ChainLink link in _links)
                link.DetachPlatforms();
            Bake();
        }

        internal static void Dispose()
        {
            if (_links == null) return;

            foreach(ChainLink link in _links)
                link.Dispose();
            _links.Clear();
        }
    }
}
