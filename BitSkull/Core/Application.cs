using BitSkull.Assets;
using BitSkull.Events;
using BitSkull.Graphics;
using BitSkull.InputSystem;
using BitSkull.Numerics;
using System;
using System.Diagnostics;
using System.IO;

namespace BitSkull.Core
{
    public class Application : IDisposable
    {
        private static Application _instance;
        private readonly string _name;
        private readonly LayerStack _layerStack;
        private BaseWindow _window;
        private static Renderer _renderer;

        public bool IsRunning { get; private set; } = false;

        public Application(string appName)
        {
            if (_instance != null) throw new Exception("Application is already created!");

            _name = appName;
            _layerStack = new LayerStack();

            _instance = this;
        }

        public static Application GetAppInstance() => _instance;
        public static Renderer GetAppRenderer() => _renderer;

        //DBG
        Renderable square;
        //

        public void Run()
        {
            _renderer.InitializeRenderQueue();
            //DBG
            {
                var square_vbo = _renderer.GenVertexBuffer(new float[]
                {
                    //x          y       u       v
                     -0.5f,    0.5f,    0.0f,   1.0f,
                     -0.5f,   -0.5f,    0.0f,   0.0f,
                      0.5f,   -0.5f,    1.0f,   0.0f,
                      0.5f,    0.5f,    1.0f,   1.0f,
                });

                square_vbo.SetLayout(new BufferLayout(new BufferElement("a_Pos", ShaderDataType.Float2), new BufferElement("a_UV", ShaderDataType.Float2)));
                square_vbo.Unbind();

                var square_ibo = _renderer.GenIndexBuffer(new uint[]
                {
                    0, 1, 2,
                    2, 3, 0
                });

                _renderer.CreateShader("testShader",
                    """
                    #version 330 core

                    layout(location = 0) in vec2 a_Pos;
                    layout(location = 1) in vec2 a_UV;

                    out vec2 v_UV;

                    uniform mat4 u_Model;
                    uniform mat4 u_View;
                    uniform mat4 u_Projection;

                    void main(){
                        v_UV = vec2(a_UV.x, -a_UV.y);
                        gl_Position = u_Model * vec4(a_Pos, 0.0, 1.0);
                    }
                    """,
                    """
                    #version 330 core

                    layout(location = 0) out vec4 frag_color;

                    in vec2 v_UV;

                    uniform sampler2D u_Texture;

                    void main(){
                        frag_color = texture(u_Texture, v_UV);
                    }
                    """,
                    new VertexShaderInfo()
                    );

                square = new Renderable(new Mesh(square_vbo, square_ibo, _renderer), new Material(_renderer.GetShader("testShader")));

                Image img = null;
                using (FileStream logo = File.OpenRead("Assets/logo.png"))
                    img = new Image(logo);
                square.Material.SaveTextureReference("mainTexture", _renderer.GenTexure2D(img));
                img.Dispose();
                square.Material.SetTexture("u_Texture", "mainTexture");

                _renderer.PushToRenderQueue("testShader", square);
                _renderer.BakeRenderQueue();
            }
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

                //DBG
                if (Input.IsKeyDown(KeyCode.W))
                    square.Transform.Position.Y += 1f * dt;
                if (Input.IsKeyDown(KeyCode.S))
                    square.Transform.Position.Y -= 1f * dt;
                if (Input.IsKeyDown(KeyCode.D))
                    square.Transform.Position.X += 1f * dt;
                if (Input.IsKeyDown(KeyCode.A))
                    square.Transform.Position.X -= 1f * dt;
                if(Input.IsKeyDown(KeyCode.Z))
                    square.Transform.Position.Z += 1f * dt;
                if (Input.IsKeyDown(KeyCode.X))
                    square.Transform.Position.Z -= 1f * dt;

                if (Input.IsKeyDown(KeyCode.Q))
                    square.Transform.Rotation.Z += Maths.DegToRad(90f) * dt;
                if (Input.IsKeyDown(KeyCode.E))
                    square.Transform.Rotation.Z -= Maths.DegToRad(90f) * dt;
                //

                if (_window != null)
                    _window.DoUpdate(dt);
                foreach (Layer layer in _layerStack)
                    layer.OnUpdate(dt);
                Input.Update();

                //rendering
                _renderer.Clear(0.3f, 0.5f, 0.78f, 1); //TOOD: camera color
                _renderer.ExecuteRenderQueue();
            }

            dtStopwatch.Stop();

            if (_renderer != null)
                _renderer.Dispose();
            if (_window != null)
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
                    _renderer.ResizeFramebuffer(resizeEvent.Width, resizeEvent.Height);
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
            _renderer = new Renderer(api, rendererCtx);
#else
            throw new Exception("Platform exception: window not found");
#endif
        }
        #endregion
    }
}
