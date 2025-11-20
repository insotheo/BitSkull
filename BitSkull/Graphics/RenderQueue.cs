using System.Collections;
using System.Collections.Generic;

namespace BitSkull.Graphics
{
    public class RenderQueue : IEnumerable
    {
        //Camera will be here...
        //Target's reference will be here...
        private List<Renderable> _renderables;
        private bool _isSorted;

        internal RenderQueue()
        {
            _renderables = new List<Renderable>();
            _isSorted = true;
        }

        public void PushRenderable(Renderable renderable)
        {
            _renderables.Add(renderable);
            _isSorted = false;
        }

        internal void Sort()
        {
            if (_isSorted) return;

            _renderables.Sort((a, b) => a.SortKey.CompareTo(b.SortKey));
            _isSorted = true;
        }

        internal void Dispose()
        {
            _renderables.Clear();
            _isSorted = true;
        }

        public IEnumerator GetEnumerator() => _renderables.GetEnumerator();
    }
}
