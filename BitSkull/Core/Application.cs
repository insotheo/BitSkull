using BitSkull.Events;
using BitSkull.Graphics;
using BitSkull.InputSystem;
using BitSkull.Numerics;
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

        //DBG
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        uint vao;
        Shader shader;
        //
        public void Run()
        {
            //DBG
            vertexBuffer = Renderer.GenVertexBuffer(new float[]
            {
                //a_Pos  x      y        z          a_Color   r      g     b
                        0.0f,   0.5f,    0.0f,               1.0f,  0.0f,  0.0f,
                        -0.5f,  -0.5f,    0.0f,              0.0f,  1.0f,  0.0f,
                        0.5f,  -0.5f,    0.0f,               0.0f,  0.0f,  1.0f,
            });
            vertexBuffer.SetLayout(new BufferLayout(new BufferElement("a_Pos", ShaderDataType.Float3), new BufferElement("a_Color", ShaderDataType.Float3)));
            vertexBuffer.Unbind();

            indexBuffer = Renderer.GenIndexBuffer(new uint[]
            {
                0, 1, 2,
            });

            vao = (Renderer.Context as Platform.OpenGL.GlRendererContext).Gl.GenVertexArray();
            (Renderer.Context as Platform.OpenGL.GlRendererContext).Gl.BindVertexArray(vao);
            vertexBuffer.Bind();
            indexBuffer.Bind();
            vertexBuffer.BindLayout();
            (Renderer.Context as Platform.OpenGL.GlRendererContext).Gl.BindVertexArray(0);
            vertexBuffer.Unbind();
            indexBuffer.Unbind();

            shader = Renderer.GenShader("""
                #version 330 core

                layout(location = 0) in vec3 a_Pos;
                layout(location = 1) in vec3 a_Color;

                out vec4 vert_Color;

                void main(){
                    vert_Color = vec4(a_Color, 1.0);
                    gl_Position = vec4(1.5 * a_Pos, 1.0);
                }
                """,
                """
                #version 330 core

                layout(location = 0) out vec4 color;

                in vec4 vert_Color;

                void main(){
                    color = vert_Color;
                }
                """);

            Log.Info("Hello, Triangle!");

            float time = 0;
            //

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
                Input.Update();

                //rendering
                //DBG
                time += dt;

                float r = (MathF.Sin(time * 0.8f) + 1f) * 0.5f;
                float g = (MathF.Sin(time * 1.5f + 2f) + 1f) * 0.5f;
                float b = (MathF.Sin(time * 1.1f + 4f) + 1f) * 0.5f;

                Renderer.Clear(r, g, b, 1);

                (Renderer.Context as Platform.OpenGL.GlRendererContext).Gl.BindVertexArray(vao);
                shader.Use();
                unsafe
                {
                    (Renderer.Context as Platform.OpenGL.GlRendererContext).Gl.DrawElements(Silk.NET.OpenGL.PrimitiveType.Triangles, indexBuffer.GetCount(), Silk.NET.OpenGL.DrawElementsType.UnsignedInt, null);
                }
                shader.ZeroUse();
                (Renderer.Context as Platform.OpenGL.GlRendererContext).Gl.BindVertexArray(0);
                //
            }

            dtStopwatch.Stop();

            //DBG
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            shader.Dispose();
            //

            if(_window != null)
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

        public void CreateWindow(int width, int heigth, string title = "", bool vsync = true, RendererApi api = RendererApi.None)
        {
            if (String.IsNullOrEmpty(title))
                title = _name;
            IRendererContext rendererCtx = null;
#if DEFAULT_PLATFORM
            _window = new Platform.GLFW.GLFWWindow(width, heigth, title, vsync, api);
            if(api == RendererApi.OpenGL)
            {
                rendererCtx = new Platform.OpenGL.GlRendererContext();
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
