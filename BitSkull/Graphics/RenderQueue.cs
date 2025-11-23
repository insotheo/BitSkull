using System.Collections;
using System.Collections.Generic;

namespace BitSkull.Graphics
{
    public class RenderQueue : IEnumerable
    {
        //Target's reference will be here...
        public Camera Camera { get; private set; }
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

        public void PushText(Text text) => PushRenderable(text.Renderable);

        public void SetCamera(Camera camera) => Camera = camera;

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
