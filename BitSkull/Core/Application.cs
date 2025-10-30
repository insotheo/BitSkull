using BitSkull.Events;
using System;

namespace BitSkull.Core
{
    public class Application : IDisposable
    {
        private static Application _inst;
        private readonly string _name;
        private readonly LayerStack _layerStack;

        public bool IsRunning { get; private set; } = false;

        public Application(string appName)
        {
            if (_inst != null) throw new Exception("Application is already created!");

            _name = appName;
            _layerStack = new LayerStack();

            _inst = this;
        }

        public static Application GetAppInstance() => _inst;

        public void Run()
        {
            IsRunning = true;
            while (IsRunning)
            {
                foreach (Layer layer in _layerStack)
                    layer.OnUpdate(1f);
            }
        }
        public void Stop() => IsRunning = false;

        public void Dispose()
        {
            Stop();
            _layerStack.Clear();

            GC.SuppressFinalize(this);
        }


        #region User interaction
        public void PushLayer(Layer layer) => _layerStack.PushLayer(layer);
        public void PushOverlay(Layer overlay) => _layerStack.PushOverlay(overlay);

        public void OnEvent(Event e)
        {
            EventDispatcher dispatcher = new EventDispatcher(e);
            dispatcher.Dispatch<AppCloseEvent>((AppCloseEvent _) =>
            { //On App close
                IsRunning = false;
                return true;
            });

            for (int i = _layerStack.Count - 1; i >= 0; --i)
            {
                _layerStack.At(i).OnEvent(e);
                if (e.Handled)
                    break;
            }
        }
        #endregion
    }
}
