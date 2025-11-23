using BitSkull.Core;
using BitSkull.Graphics;
using Silk.NET.Core.Contexts;
using Silk.NET.Input;
using Silk.NET.Input.Sdl;
using Silk.NET.SDL;

namespace BitSkull.Platform.SDL
{
    internal unsafe sealed class SDLWindow : BaseWindow
    {
        Sdl _sdl;
        Window* _sdlWindow;

        public SDLWindow(int width, int height, string title, RendererApi api, bool vsync = false)
        {
            _sdl = Sdl.GetApi();

            if(_sdl.Init(Sdl.InitVideo) < 0)
            {
                Log.Error("SDL video initialization failed!");
                return;
            }

            if (api == RendererApi.OpenGL)
            {
                _sdl.GLSetAttribute(GLattr.ContextMajorVersion, 4);
                _sdl.GLSetAttribute(GLattr.ContextMinorVersion, 6);
                _sdl.GLSetAttribute(GLattr.ContextProfileMask, (int)GLprofile.Core);
                _sdl.GLSetAttribute(GLattr.Doublebuffer, 1);
            }

            Width = width;
            Height = height;
            Title = title;

            _sdlWindow = _sdl.CreateWindow(
                title,
                Sdl.WindowposUndefined, Sdl.WindowposUndefined,
                Width, Height,
                (uint)WindowFlags.Resizable | (uint)(api == RendererApi.OpenGL ? WindowFlags.Opengl : 0)
                );

            if(_sdlWindow == null)
            {
                Log.Error("SDL window failed to initialize!");
                return;
            }

            if(api == RendererApi.OpenGL)
            {
                if(_sdl.GLCreateContext(_sdlWindow) == null)
                    Log.Error("Failed to create gl context");
            }

            SetVSync(vsync);
        }

        internal override void Run() => _sdl.ShowWindow(_sdlWindow);
        internal override void Close() => Application.GetAppInstance().OnEvent(new Events.AppCloseEvent());

        internal override void SetVSync(bool vsync)
        {
            VSync = vsync;
            _sdl.GLSetSwapInterval(vsync ? 1 : 0);
        }

        internal override void DoUpdate(float dt)
        {
            PollEvents();
            _sdl.GLSwapWindow(_sdlWindow);
        }

        private void PollEvents()
        {
            Application app = Application.GetAppInstance();
            Event ev;

            while(_sdl.PollEvent(&ev) != 0)
            {
                switch ((EventType)ev.Type)
                {
                    case EventType.Quit:
                        app.OnEvent(new Events.AppCloseEvent());
                        break;

                    case EventType.Keydown:
                        app.OnEvent(new Events.KeyPressedEvent((InputSystem.KeyCode)(Key)ev.Key.Keysym.Sym));
                        break;

                    case EventType.Keyup:
                        app.OnEvent(new Events.KeyReleasedEvent((InputSystem.KeyCode)(Key)ev.Key.Keysym.Sym));
                        break;

                    case EventType.Mousebuttondown:
                        app.OnEvent(new Events.MouseButtonPressed((InputSystem.MouseButton)(MouseButton)ev.Button.Button));
                        break;

                    case EventType.Mousebuttonup:
                        app.OnEvent(new Events.MouseButtonReleased((InputSystem.MouseButton)(MouseButton)ev.Button.Button));
                        break;

                    case EventType.Mousemotion:
                        app.OnEvent(new Events.MouseMovedEvent(ev.Motion.X, ev.Motion.Y));
                        break;

                    case EventType.Mousewheel:
                        app.OnEvent(new Events.MouseScrollEvent(ev.Wheel.X, ev.Wheel.Y));
                        break;

                    case EventType.Windowevent:
                        HandleWindowEvent(ev, app);
                        break;

                    default: break;
                }
            }
        }

        private void HandleWindowEvent(Event ev, Application app)
        {
            switch ((WindowEventID)ev.Window.Event)
            {
                case WindowEventID.Resized:
                case WindowEventID.SizeChanged:
                    {
                        Width = ev.Window.Data1;
                        Height = ev.Window.Data2;
                        app.OnEvent(new Events.WindowResizeEvent(Width, Height));
                    }
                    break;

                case WindowEventID.Moved:
                    app.OnEvent(new Events.WindowMoveEvent(ev.Window.Data1, ev.Window.Data2));
                    break;

                case WindowEventID.FocusGained:
                case WindowEventID.FocusLost:
                    IsFocused = ((WindowEventID)ev.Window.Event == WindowEventID.FocusGained);
                    app.OnEvent(new Events.WindowFocusEvent(IsFocused));
                    break;

                default: break;
            }
        }

        internal override INativeContext GetContext()
        {
            SdlContext ctx = new SdlContext(_sdl, _sdlWindow);
            ctx.Create();
            return ctx;
        }

        public override void Dispose()
        {
            if (_sdlWindow != null)
                _sdl.DestroyWindow(_sdlWindow);

            _sdl.Quit();
        }
    }
}
