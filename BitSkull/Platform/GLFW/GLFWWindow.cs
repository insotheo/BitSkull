using BitSkull.Core;
using BitSkull.Events;
using Silk.NET.GLFW;
using System;

namespace BitSkull.Platform.GLFW
{
    internal unsafe sealed class GLFWWindow : BaseWindow
    {
        private readonly Glfw _glfw;

        private readonly WindowHandle* _glfwWindow;

        internal GLFWWindow(int width, int height, string title, bool vsync = false)
        {
            _glfw = Glfw.GetApi();
            if (!_glfw.Init())
                throw new Exception("Failed to initialize GLFW");

            //Just for now
            _glfw.DefaultWindowHints();

            Width = width;
            Height = height;
            Title = title;

            _glfwWindow = _glfw.CreateWindow(Width, Height, Title, null, null);
            if (_glfwWindow == null)
            {
                _glfw.Terminate();
                throw new Exception("Failed to create window!");
            }
            _glfw.MakeContextCurrent(_glfwWindow);
            SetVSync(vsync);


            //Events
            _glfw.SetWindowCloseCallback(_glfwWindow, (WindowHandle* _wnd) =>
            {
                Application.GetAppInstance().OnEvent(new AppCloseEvent());
            });
        }

        internal override void Run() => _glfw.ShowWindow(_glfwWindow);
        internal override void Close() => _glfw.SetWindowShouldClose(_glfwWindow, true);
        internal override void SetVSync(bool vsync)
        {
            VSync = vsync;
            _glfw.SwapInterval(vsync ? 1 : 0);
        }

        internal override void DoUpdate(float dt)
        {
            _glfw.PollEvents();
            _glfw.SwapBuffers(_glfwWindow);
            //Input update for sure
        }

        public override void Dispose()
        {
            if (!_glfw.WindowShouldClose(_glfwWindow))
                Close();

            _glfw.DestroyWindow(_glfwWindow);
            _glfw.Terminate();
            _glfw.Dispose();
        }
    }
}
