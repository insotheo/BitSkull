using System.Collections;
using System.Collections.Generic;

namespace BitSkull.Graphics
{
    public class RenderQueue : IEnumerable
    {
        private List<Renderable> _renderables;
        private bool _isSorted;

        public RenderQueue()
        {
            _renderables = new List<Renderable>();
            _isSorted = true;
        }

        public void PushRenderable(Renderable renderable)
        {
            _renderables.Add(renderable);
            _isSorted = false;
        }

        public void PopRenderable(Renderable renderable)
        {
            _renderables.Remove(renderable);
        }

        public void Sort()
        {
            if (_isSorted) return;

            _renderables.Sort((a, b) => a.SortKey.CompareTo(b.SortKey));
            _isSorted = true;
        }

        public void BakeAll(Renderer renderer)
        {
            foreach (Renderable renderable in _renderables)
                renderable.Bake(renderer);
        }

        public void DisposeAndClear()
        {
            foreach (Renderable renderable in _renderables)
                renderable.Mesh.Dispose();
            Clear();
        }
        public void Clear()
        {
            _renderables.Clear();
            _isSorted = true;
        }

        public IEnumerator GetEnumerator() => _renderables.GetEnumerator();
    }
}
