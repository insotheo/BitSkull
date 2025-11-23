using BitSkull.Assets;
using BitSkull.Events;
using BitSkull.Graphics;
using BitSkull.InputSystem;
using BitSkull.Numerics;
using System;
using System.Collections.Generic;
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

        public (int width, int height) GetWindowSize() => (_window.Width, _window.Height);

        Camera cam;
        Font font;

        public void Run()
        {
            using (FileStream fs = File.OpenRead("Assets/Roboto.ttf"))
                font = new Font(fs, 64);
            _renderer.GenFontTexture(font);

            _renderer.CreateShader("textDefaultShader",
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
                """,
                """
                #version 330 core

                in vec2 v_UV;

                out vec4 frag_Color;

                uniform sampler2D u_FontTexture;
                uniform vec4 u_Color;

                void main(){
                    float alpha = texture(u_FontTexture, v_UV).r;
                    frag_Color = vec4(u_Color.rgb, alpha);
                    //frag_Color = vec4(u_Color.rgb, 1.0);
                }
                """,
                new VertexShaderInfo()
            );

            Text text = new Text(_renderer, 
                new BufferLayout(new BufferElement("a_Pos", ShaderDataType.Float3), new BufferElement("a_UV", ShaderDataType.Float2)), 
                new Renderable(new Mesh(null, null, _renderer), new Material(_renderer.GetShader("textDefaultShader"))), 
                font, 
                content: $"Hello, {Environment.UserName}!",
                nativeScale: 100f);
            Color4 textColor = new Color4(1f, 1f, 1f);

            text.Renderable.Material.SetTexture("u_FontTexture", font.FontTexture);
            text.Renderable.Material.SetColor("u_Color", textColor);
            text.Renderable.Transform.Rotation.Y = Maths.DegToRad(45f);
            text.Renderable.Transform.Rotation.X = Maths.DegToRad(-30f);
            int counter = 0;

            Log.Info("Hold [R] for magic");
            cam = new Camera(CameraType.Perspective);
            cam.Position.Z = 1.5f;

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
                    cam.Position.Y += 2f * dt;
                if (Input.IsKeyDown(KeyCode.S))
                    cam.Position.Y -= 2f * dt;
                if (Input.IsKeyDown(KeyCode.D))
                    cam.Position.X += 2f * dt;
                if (Input.IsKeyDown(KeyCode.A))
                    cam.Position.X -= 2f * dt;
                if (Input.IsKeyDown(KeyCode.Z))
                    cam.Position.Z += 2f * dt;
                if (Input.IsKeyDown(KeyCode.X))
                    cam.Position.Z -= 2f * dt;

                if (Input.IsKeyDown(KeyCode.R))
                {
                    counter += 1;
                    textColor.R = (MathF.Sin(counter/10f * 0.8f) + 1f) * 0.5f;
                    textColor.G = (MathF.Sin(counter/10f * 1.5f + 2f) + 1f) * 0.5f;
                    textColor.B = (MathF.Sin(counter/10f * 1.1f + 4f) + 1f) * 0.5f;
                    Log.Trace($"Score: {counter}");
                    text.SetContent($"Score: {counter}");
                    text.Renderable.Material.SetColor("u_Color", textColor);
                }

                if (_window != null)
                    _window.DoUpdate(dt);
                foreach (Layer layer in _layerStack)
                    layer.OnUpdate(dt);
                Input.Update();

                //rendering
                _renderer.BeginFrame();
                RenderQueue mainQueue = _renderer.CreateQueue();
                mainQueue.SetCamera(cam);

                mainQueue.PushText(text);

                _renderer.EndFrame();
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
