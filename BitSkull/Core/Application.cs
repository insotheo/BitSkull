using BitSkull.Events;
using BitSkull.Graphics;
using BitSkull.Graphics.Queue;
using BitSkull.InputSystem;
using BitSkull.Numerics;
using System;
using System.Diagnostics;

namespace BitSkull.Core
{
    public class Application : IDisposable
    {
        private static Application _instance;
        private readonly string _name;
        private readonly LayerStack _layerStack;
        private BaseWindow _window;

        public bool IsRunning { get; private set; } = false;

        public Application(string appName)
        {
            if (_instance != null) throw new Exception("Application is already created!");

            _name = appName;
            _layerStack = new LayerStack();

            _instance = this;
        }

        public static Application GetAppInstance() => _instance;

        //DBG
        Renderable square;
        //

        public void Run()
        {
            //DBG
            {
                var square_vbo = Renderer.GenVertexBuffer(new float[]
                {
                    //a_Pos  x            y
                            -0.75f,       0.75f,
                            -0.75f,      -0.75f,
                             0.75f,      -0.75f,
                             0.75f,       0.75f
                });

                square_vbo.SetLayout(new BufferLayout(new BufferElement("a_Pos", ShaderDataType.Float2)));
                square_vbo.Unbind();

                var square_ibo = Renderer.GenIndexBuffer(new uint[]
                {
                    0, 1, 2,
                    2, 3, 0
                });

                var shader = Renderer.GenShader("""
                    #version 330 core

                    layout(location = 0) in vec2 a_Pos;

                    out vec2 v_Pos;

                    void main(){
                        v_Pos = vec2(a_Pos.x, -a_Pos.y);
                        gl_Position = vec4(a_Pos, 0.0, 1.0);
                    }
                    """,
                    """
                    #version 330 core

                    layout(location = 0) out vec4 frag_color;

                    in vec2 v_Pos;

                    uniform float u_Time;
                    uniform float u_Radius = 0.1f;
                    uniform vec2 u_MousePos;

                    void main(){
                        vec2 uv = v_Pos * 0.5 + 0.5;
                        float dist = distance(uv, u_MousePos);

                        float r = (sin(u_Time * 0.8) + 1.0) * 0.5;
                        float g = (sin(u_Time * 1.5 + 2.0) + 1.0) * 0.5;
                        float b = (sin(u_Time * 1.1 + 4.0) + 1.0) * 0.5;
                        vec3 baseColor = vec3(r, g, b);

                        //circle effect
                        float circleMask = smoothstep(u_Radius, u_Radius - 0.02, dist);
                        vec3 finalColor = mix(baseColor, vec3(0.4), 1.0 - circleMask);

                        frag_color = vec4(finalColor, 1.0);
                    }
                    """
                    );

                square = new Renderable(square_vbo, square_ibo, shader);

                RenderQueue.Push(square);
                RenderQueue.Bake();
            }
            //

            IsRunning = true;
            if (_window != null)
                _window.Run();

            Stopwatch dtStopwatch = new Stopwatch();//dt is delta time
            dtStopwatch.Start();
            double prevTime = dtStopwatch.Elapsed.TotalSeconds, currTime = 0.0;
            float dt = 0.0f;

            float time = dt;
            while (IsRunning)
            {
                currTime = dtStopwatch.Elapsed.TotalSeconds;
                dt = (float)(currTime - prevTime);
                prevTime = currTime;

                //update
                if (_window != null)
                    _window.DoUpdate(dt);
                foreach (Layer layer in _layerStack)
                    layer.OnUpdate(dt);
                Input.Update();

                //rendering
                Renderer.Clear(0.3f, 0.5f, 0.78f, 1);
                Renderer.ExecuteRenderQueue();

                time += dt;
                square.Material.SetReal("u_Time", time * 2f);

                Vec2D mousePos = Input.GetMousePos();
                square.Material.SetVec2D("u_MousePos", new(mousePos.X / (float)_window.Width, mousePos.Y / (float)_window.Height));
            }

            dtStopwatch.Stop();

            RenderQueue.Dispose();
            if (_window != null)
            {
                _window.Dispose();
                _window = null;
            }
            Renderer.Dispose();
        }
        public void Stop() => IsRunning = false;

        public void Dispose()
        {
            Stop();
            _layerStack.Clear();

            _instance = null;
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
            dispatcher.Dispatch<KeyboardEvent>((KeyboardEvent keyboardEvent) =>
            {
                switch (keyboardEvent)
                {
                    case KeyPressedEvent pressedEvent:
                        Input.OnKeyDown(pressedEvent.KeyCode);
                        break;

                    case KeyReleasedEvent releasedEvent:
                        Input.OnKeyUp(releasedEvent.KeyCode);
                        break;

                    case KeyTypedEvent typedEvent: //push it to the next layer
                    default: return false;
                }
                return true; //captured by Input class
            });
            dispatcher.Dispatch<MouseEvent>((MouseEvent mouseEvent) =>
            {
                switch (mouseEvent)
                {
                    case MouseButtonPressed pressedEvent:
                        Input.OnMouseDown(pressedEvent.Button);
                        break;

                    case MouseButtonReleased releasedEvent:
                        Input.OnMouseUp(releasedEvent.Button);
                        break;

                    case MouseMovedEvent movedEvent:
                        Input.OnMouseMove(new Vec2D(movedEvent.X, movedEvent.Y));
                        break;

                    case MouseScrollEvent scrollEvent:
                        Input.OnMouseScroll(new Vec2D(scrollEvent.XOffset, scrollEvent.YOffset));
                        break;

                    default: return false;
                }
                return true;
            });
            dispatcher.Dispatch<WindowEvent>((WindowEvent wndEvent) =>
            {
                if (wndEvent is WindowResizeEvent resizeEvent)
                    Renderer.ResizeFramebuffer(resizeEvent.Width, resizeEvent.Height);
                return false;
            });

            for (int i = _layerStack.Count - 1; i >= 0; --i)
            {
                _layerStack.At(i).OnEvent(e);
                if (e.Handled)
                    break;
            }
        }

        public void CreateWindow(int width, int height, string title = "", bool vsync = true, RendererApi api = RendererApi.None)
        {
            if (String.IsNullOrEmpty(title))
                title = _name;
            IRenderBackend rendererCtx = null;
#if DEFAULT_PLATFORM
            _window = new Platform.GLFW.GLFWWindow(width, height, title, vsync, api);
            if (api == RendererApi.OpenGL)
            {
                rendererCtx = new Platform.OpenGL.OpenGLBackend();
                rendererCtx.Initialize(_window.GetContext());
            }
            Renderer.Init(api, rendererCtx);
#else
            throw new Exception("Platform exception: window not found");
#endif
        }
        #endregion
    }
}
