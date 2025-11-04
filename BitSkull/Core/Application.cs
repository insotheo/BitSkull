using BitSkull.Events;
using System;
using System.Diagnostics;

namespace BitSkull.Core
{
    public class Application : IDisposable
    {
        private static Application _inst;
        private readonly string _name;
        private readonly LayerStack _layerStack;
        private BaseWindow _window;

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
            if (_window != null)
                _window.Run();

            Stopwatch dtStopwatch = new Stopwatch();//dt is delta time
            dtStopwatch.Start();
            double prevTime = dtStopwatch.Elapsed.TotalSeconds, currTime = 0.0;
            float dt = 0.0f;

            while (IsRunning)
            {
                currTime = dtStopwatch.Elapsed.TotalSeconds;
                dt = (float)(currTime - prevTime);
                prevTime = currTime;

                //update
                if(_window != null)
                    _window.DoUpdate(dt);
                foreach (Layer layer in _layerStack)
                    layer.OnUpdate(dt);
            }

            dtStopwatch.Stop();

            if(_window != null)
            {
                _window.Dispose();
                _window = null;
            }
        }
        public void Stop() => IsRunning = false;

        public void Dispose()
        {
            Stop();
            _layerStack.Clear();

            _inst = null;
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

        public void CreateWindow(int width, int heigth, string title = "", bool vsync = true)
        {
            if (String.IsNullOrEmpty(title))
                title = _name;
#if DEFAULT_PLATFORM
            _window = new Platform.GLFW.GLFWWindow(width, heigth, title, vsync);
#else
            throw new Exception("Platform exception: window not found");
#endif
        }
        #endregion
    }
}
