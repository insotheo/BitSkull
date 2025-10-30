using Silk.NET.Windowing;
using Silk.NET.Maths;
using BitSkull.Core;
using BitSkull.Events;
using System;

namespace BitSkull.Platform.SilkNet
{
    public sealed class SilkWindow : BaseWindow, IDisposable
    {
        private IWindow _windowInstance;

        public SilkWindow(int width, int height, string title, bool vsync = true)
        {
            Width = width;
            Height = height;
            Title = title;
            VSync = vsync;

            //Window definition
            WindowOptions options = WindowOptions.Default;
            options.Size = new Vector2D<int>(width, height);
            options.Title = title;

            _windowInstance = Window.Create(options);

            _windowInstance.Closing += () =>
            { //Window closing
                Application.GetAppInstance().OnEvent(new AppCloseEvent());
            };
        }



        internal override void Run() => _windowInstance.Run();
        internal override void Close() => _windowInstance.Close();
        internal override void SetVSync(bool vsync) => _windowInstance.VSync = vsync;

        public void Dispose()
        {
            if(_windowInstance != null)
                _windowInstance.Dispose();
        }
    }
}
