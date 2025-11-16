using BitSkull.Core;
using BitSkull.Events;
using BitSkull.Graphics;
using BitSkull.InputSystem;
using Silk.NET.Core.Contexts;
using Silk.NET.GLFW;
using Silk.NET.Input;
using System;

namespace BitSkull.Platform.GLFW
{
    internal unsafe sealed class GLFWWindow : BaseWindow
    {
        private readonly Glfw _glfw;

        private readonly WindowHandle* _glfwWindow;

        internal GLFWWindow(int width, int height, string title, bool vsync = false, RendererApi api = RendererApi.OpenGL)
        {
            _glfw = Glfw.GetApi();
            if (!_glfw.Init())
                throw new Exception("Failed to initialize GLFW");

            if(api == RendererApi.OpenGL)
            {
                _glfw.WindowHint(WindowHintInt.ContextVersionMajor, 4);
                _glfw.WindowHint(WindowHintInt.ContextVersionMinor, 6);
                _glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
                if(Environment.OSVersion.Platform == PlatformID.Unix)
                    _glfw.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
            }

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


            ////////////////////////////////////////////////////////////////////////Events
            _glfw.SetWindowCloseCallback(_glfwWindow, (WindowHandle* wnd) =>
            {
                Application.GetAppInstance().OnEvent(new AppCloseEvent());
            });

            //Input
            _glfw.SetKeyCallback(_glfwWindow, (WindowHandle* wnd, Keys key, int scancode, InputAction action, KeyModifiers mods) =>
            {
                switch (action)
                {
                    case InputAction.Press:
                        Application.GetAppInstance().OnEvent(new KeyPressedEvent((KeyCode)(Key)key));
                        break;

                    case InputAction.Release:
                        Application.GetAppInstance().OnEvent(new KeyReleasedEvent((KeyCode)(Key)key));
                        break;

                    default: break;
                }
            });
            _glfw.SetCharCallback(_glfwWindow, (WindowHandle* wnd, uint codepoint) =>
            {
                Application.GetAppInstance().OnEvent(new KeyTypedEvent((char)codepoint));
            });
            _glfw.SetMouseButtonCallback(_glfwWindow, (WindowHandle* wnd, Silk.NET.GLFW.MouseButton btn, InputAction action, KeyModifiers mods) =>
            {
                switch (action)
                {
                    case InputAction.Press:
                        Application.GetAppInstance().OnEvent(new MouseButtonPressed((InputSystem.MouseButton)(Silk.NET.Input.MouseButton)btn));
                        break;

                    case InputAction.Release:
                        Application.GetAppInstance().OnEvent(new MouseButtonReleased((InputSystem.MouseButton)(Silk.NET.Input.MouseButton)btn));
                        break;

                    default: break;
                }
            });
            _glfw.SetCursorPosCallback(_glfwWindow, (WindowHandle* wnd, double x, double y) =>
            {
                Application.GetAppInstance().OnEvent(new MouseMovedEvent((float)x, (float)y));
            });
            _glfw.SetScrollCallback(_glfwWindow, (WindowHandle* wnd, double x, double y) =>
            {
                Application.GetAppInstance().OnEvent(new MouseScrollEvent((float)x, (float)y));
            });

            //Window state
            _glfw.SetWindowPosCallback(_glfwWindow, (WindowHandle* wnd, int x, int y) =>
            {
                Application.GetAppInstance().OnEvent(new WindowMoveEvent(x, y));
            });
            _glfw.SetWindowSizeCallback(_glfwWindow, (WindowHandle* wnd, int x, int y) =>
            {
                Width = x;
                Height = y;
                Application.GetAppInstance().OnEvent(new WindowResizeEvent(Width, Height));
            });
            _glfw.SetWindowFocusCallback(_glfwWindow, (WindowHandle* wnd, bool focus) =>
            {
                IsFocused = focus;
                Application.GetAppInstance().OnEvent(new WindowFocusEvent(focus));
            });
            ////////////////////////////////////////////////////////////////////////
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
        }

        internal override INativeContext GetContext()
        {
            if (_glfwWindow != null) return new GlfwContext(_glfw, _glfwWindow);
            return null;
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
