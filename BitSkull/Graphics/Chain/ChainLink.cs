using System;
using System.Collections.Generic;

namespace BitSkull.Graphics.Chain
{
    public class ChainLink : IDisposable
    {
        internal List<VertexBuffer> VBuffers { get; private set; }
        internal List<IndexBuffer> IBuffers { get; private set; }
        internal uint[] IndicesCounts { get; private set; }

        internal readonly Shader Shader;


        internal List<IPlatformChainLink> Platforms { get; private set; } = null;
        internal bool HasPlatform => Platforms != null && Platforms.Count > 0;
        private int _bakedCount = 0;

        internal ChainLink(VertexBuffer vbo, IndexBuffer ibo, Shader shader = null)
        {
            VBuffers = new() { vbo };
            IBuffers = new() { ibo };
            Platforms = new();
            Shader = shader;
        }

        internal void Bake(bool force = false)
        {
            if (_bakedCount == Platforms.Count && _bakedCount != 0 && !force)
                return;

            DetachPlatforms();
            if (VBuffers.Count != IBuffers.Count && VBuffers.Count != 0 && IBuffers.Count != 0)
                return;

            List<uint> counts = new();
            for (int i = 0; i < VBuffers.Count; i++)
            {
                IPlatformChainLink platform = Renderer.Context.GenPlatformChainLink(VBuffers[i], IBuffers[i]);
                Platforms.Add(platform);
                counts.Add(IBuffers[i].GetCount());
            }

            IndicesCounts = counts.ToArray();
            counts.Clear();

            _bakedCount = Platforms.Count;
        }

        internal void AttachChainLink(ChainLink link)
        {
            VBuffers.AddRange(link.VBuffers);
            IBuffers.AddRange(link.IBuffers);
            Platforms.AddRange(link.Platforms);
            if (IndicesCounts != null && link.IndicesCounts != null)
            {
                uint[] tmp = new uint[IndicesCounts.Length + link.IndicesCounts.Length];
                IndicesCounts.CopyTo(tmp, 0);
                for (int i = IndicesCounts.Length; i < tmp.Length; i++)
                    tmp[i] = link.IndicesCounts[i - IndicesCounts.Length];
                tmp.CopyTo(IndicesCounts, 0);
            }
        }

        internal void DetachPlatforms()
        {
            if (Platforms == null)
                return;

            foreach (IPlatformChainLink platform in Platforms)
                platform.Dispose();
            Platforms.Clear();
            _bakedCount = 0;
        }

        public void Dispose()
        {
            DetachPlatforms();
            Platforms = null;

            Shader?.Dispose();

            foreach (VertexBuffer vbo in VBuffers)
                vbo.Dispose();
            VBuffers.Clear();
            VBuffers = null;

            foreach (IndexBuffer ibo in IBuffers)
                ibo.Dispose();
            IBuffers.Clear();
            IBuffers = null;
        }
    }
}
