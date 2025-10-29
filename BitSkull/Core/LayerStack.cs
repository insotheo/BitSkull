using System;
using System.Collections;
using System.Collections.Generic;

namespace BitSkull.Core
{
    public sealed class LayerStack : IEnumerable<Layer>
    {
        private readonly List<Layer> _layers;
        private int _layerInsertIdx = 0;
        public int Count => _layers.Count;

        internal LayerStack()
        {
            _layers = new List<Layer>();
        }

        public void PushLayer(Layer layer)
        {
            _layers.Insert(_layerInsertIdx, layer);
            _layerInsertIdx += 1;
            layer.OnAttach();
        }

        public void PushOverlay(Layer overlay)
        {
            _layers.Add(overlay);
            overlay.OnAttach();
        }

        public void PopLayer(Layer layer)
        {
            if (!_layers.Contains(layer)) return;

            _layers.Remove(layer);
            _layerInsertIdx = Math.Max(0, _layerInsertIdx - 1);
            layer.OnDetach();
        }

        public void PopOverlay(Layer overlay)
        {
            if (!_layers.Contains(overlay)) return;

            _layers.Remove(overlay);
            overlay.OnDetach();
        }


        public bool HasLayer(string name)
        {
            foreach (Layer layer in _layers)
            {
                if (layer.Name == name)
                    return true;
            }
            return false;
        }

        public Layer GetLayer(string name)
        {
            foreach (Layer layer in _layers)
            {
                if (layer.Name == name)
                    return layer;
            }
            return null;
        }

        internal void Clear()
        {
            foreach (Layer layer in _layers)
                layer.OnDetach();
            _layers.Clear();
            _layerInsertIdx = 0;
        }

        public IEnumerator<Layer> GetEnumerator() => _layers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
