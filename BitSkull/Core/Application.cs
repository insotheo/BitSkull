using BitSkull.Assets;
using BitSkull.Events;
using BitSkull.Graphics;
using BitSkull.InputSystem;
using BitSkull.Numerics;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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

        public (int width, int height) GetWindowSize() => (_window.Width, _window.Height);

        Camera cam;
        Camera uiCam;
        Renderable sprite;
        Renderable logo;

        Font font;

        public void Run()
        {
            Image img;
            var imgs = Directory.GetFiles(Path.Combine("C:\\Users\\", Environment.UserName, "Pictures"), "*.jpg");
            using (FileStream fs = File.OpenRead(imgs[new Random().Next(0, imgs.Length - 1)]))
                img = new Image(fs);

            Image imgLogo;
            using(FileStream fs = File.OpenRead("Assets/logo.png"))
                imgLogo = new Image(fs);

            {
                using (FileStream fs = File.OpenRead("Assets/Roboto.ttf"))
                    font = new Font(fs, 64);

                string vertexShaderSrc =
                    """
                    #version 330 core

                    layout(location = 0) in vec3 a_Pos;
                    layout(location = 1) in vec2 a_UV;

                    out vec2 v_UV;
                
                    uniform mat4 u_Model;
                    uniform mat4 u_View;
                    uniform mat4 u_Projection;

                    void main(){
                        v_UV = vec2(a_UV.x, a_UV.y);
                        gl_Position = u_Projection * u_View * u_Model * vec4(a_Pos, 1.0);
                    }
                    """;
                BufferLayout layout = new BufferLayout(new("a_Pos", ShaderDataType.Float3), new("a_UV", ShaderDataType.Float2));
                VertexShaderInfo vertexShaderInfo = new();
                _renderer.CreateShader("textDefaultShader",
                    vertexShaderSrc,
                    """
                    #version 330 core

                    in vec2 v_UV;

                    out vec4 frag_Color;

                    uniform sampler2D u_FontTexture;
                    uniform vec3 u_Color;

                    void main(){
                        float alpha = texture(u_FontTexture, v_UV).r;
                        frag_Color = vec4(u_Color, alpha);
                    }
                    """,
                    layout,
                    vertexShaderInfo
                );
                _renderer.CreateShader("spriteBaseShader",
                    vertexShaderSrc,
                    """
                    #version 330 core
                    
                    in vec2 v_UV;
                    
                    out vec4 frag_Color;
                    
                    uniform sampler2D u_Texture;
                    uniform float u_Alpha = 1.0;

                    void main(){
                        frag_Color = vec4(texture(u_Texture, v_UV).rgb, u_Alpha);
                    }
                    """,
                    layout,
                    vertexShaderInfo);

                VertexBuffer vbo = _renderer.GenVertexBuffer(new float[]{
                 //     x       y       z       u       v
                       -1f,    1f,     0f,      0f,     0f,
                       -1f,   -1f,     0f,      0f,     1f,
                        1f,   -1f,     0f,      1f,     1f,
                        1f,    1f,     0f,      1f,     0f,
                });
                vbo.SetLayout(layout);
                IndexBuffer ibo = _renderer.GenIndexBuffer(new uint[]
                {
                    0, 1, 2,
                    2, 3, 0
                });
                Mesh mesh = _renderer.CreateMesh(vbo, ibo);

                sprite = new Renderable(mesh, new Material(_renderer.GetShader("spriteBaseShader")));
                logo = new Renderable(mesh, new Material(_renderer.GetShader("spriteBaseShader")));

                Texture2D t = _renderer.GenTexture2D(img);
                sprite.Material.SetTexture("u_Texture", t);

                Texture2D t2 = _renderer.GenTexture2D(imgLogo);
                logo.Material.SetTexture("u_Texture", t2);
            }
            Text text = _renderer.CreateText("Hello, World!", font, "textDefaultShader", nativeScale: 500f);
            text.Renderable.Material.SetTexture("u_FontTexture", font.FontTexture);
            text.Renderable.Material.SetColor("u_Color", new Color3(0.8f, 0.8f, 0.3f));

            cam = new Camera(CameraType.Orthographic);
            cam.Zoom = 5f;
            uiCam = new Camera(CameraType.Orthographic);
            uiCam.Zoom = 2.5f;
            text.Transform.Position.Y = uiCam.Position.Y + 0.4f * uiCam.Zoom;
            text.Transform.Position.X = uiCam.Position.X - 0.7f * uiCam.Zoom;

            logo.Transform.Scale *= 0.25f;
            logo.Transform.Position.Y = uiCam.Position.Y - 0.4f * uiCam.Zoom;
            logo.Transform.Position.X = uiCam.Position.X + 0.6f * uiCam.Zoom;

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
                if (Input.IsKeyDown(KeyCode.W))
                    sprite.Transform.Position.Y += 2f * dt;
                if (Input.IsKeyDown(KeyCode.S))
                    sprite.Transform.Position.Y -= 2f * dt;
                if (Input.IsKeyDown(KeyCode.D))
                    sprite.Transform.Position.X += 2f * dt;
                if (Input.IsKeyDown(KeyCode.A))
                    sprite.Transform.Position.X -= 2f * dt;
                if (Input.IsKeyDown(KeyCode.Z))
                    sprite.Transform.Position.Z += 2f * dt;
                if (Input.IsKeyDown(KeyCode.X))
                    sprite.Transform.Position.Z -= 2f * dt;

                if (_window != null)
                    _window.DoUpdate(dt);
                foreach (Layer layer in _layerStack)
                    layer.OnUpdate(dt);
                Input.Update();

                //rendering
                _renderer.BeginFrame();

                RenderQueue uiQueue = _renderer.CreateQueue();
                uiQueue.SetCamera(uiCam);
                uiQueue.PushText(text);
                uiQueue.PushRenderable(logo);
                logo.Material.SetReal("u_Alpha", 0.25f);

                RenderQueue mainQueue = _renderer.CreateQueue();
                mainQueue.SetCamera(cam);
                mainQueue.PushRenderable(sprite);
                sprite.Material.SetReal("u_Alpha", 1f);

                _renderer.EndFrame(cam);

                text.SetContent($"FPS: {MathF.Round(1 / dt)}");
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
