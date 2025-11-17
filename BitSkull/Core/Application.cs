using BitSkull.Events;
using BitSkull.Graphics;
using BitSkull.Graphics.Chain;
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

        public void Run()
        {
            //DBG
            {
                var square_vbo = Renderer.GenVertexBuffer(new float[]
                {
                    //a_Pos  x      y        z          a_Color   r      g     b
                            -0.5f,   0.5f,   0.0f,               1.0f,  0.0f,  0.0f,
                            -0.5f,  -0.5f,   0.0f,               0.0f,  1.0f,  0.0f,
                            0.5f,  -0.5f,    0.0f,               0.0f,  0.0f,  1.0f,
                            0.5f,  0.5f,     0.0f,               0.5f,  0.0f,  0.5f,
                });
                square_vbo.SetLayout(new BufferLayout(new BufferElement("a_Pos", ShaderDataType.Float3), new BufferElement("a_Color", ShaderDataType.Float3)));
                square_vbo.Unbind();

                var square_ibo = Renderer.GenIndexBuffer(new uint[]
                {
                    0, 1, 2,
                    2, 3, 0
                });

                var triangle_vbo = Renderer.GenVertexBuffer(new float[]
                {
                    //a_Pos  x      y        z          a_Color   r      g     b
                            0.0f,   0.25f,   0.0f,                1.0f,  1.0f,  1.0f,
                            -0.25f,  -0.25f,   0.0f,               1.0f,  1.0f,  1.0f,
                            0.25f,  -0.25f,    0.0f,               1.0f,  1.0f,  1.0f,
                });
                triangle_vbo.SetLayout(new BufferLayout(new BufferElement("a_Pos", ShaderDataType.Float3), new BufferElement("a_Color", ShaderDataType.Float3)));
                triangle_vbo.Unbind();

                var triangle_ibo = Renderer.GenIndexBuffer(new uint[]
                {
                    0, 1, 2,
                });

                var hex_vbo = Renderer.GenVertexBuffer(new float[]
                {
                    //x          y
                    0.0f,       0.0f,
                    -0.35f,     0.7f,
                    -0.7f,      0.0f,
                    -0.35f,     -0.7f,
                     0.35f,     -0.7f,
                     0.7f,       0.0f,
                     0.35f,      0.7f,
                });
                hex_vbo.SetLayout(new BufferLayout(new BufferElement("a_Pos", ShaderDataType.Float2)));
                hex_vbo.Unbind();

                var hex_ibo = Renderer.GenIndexBuffer(new uint[]
                {
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 4,
                    0, 4, 5,
                    0, 5, 6,
                    0, 6, 1,
                });
                hex_ibo.Unbind();

                var fish_eye_shader = Renderer.GenShader("""
                    #version 330 core

                    layout(location = 0) in vec3 a_Pos;
                    layout(location = 1) in vec3 a_Color;

                    out vec4 vert_Color;

                    void main(){
                        vert_Color = vec4(a_Color, 1.0);

                        vec2 pos = a_Pos.xy;
                        pos.x -= 0.25;
                        pos.y -= 0.3;

                        float r = length(pos);

                        //fisheye
                        float factor = 1.0 + 0.75 * r * r;
                        vec2 fishPos = pos / factor;

                        //ripple effect
                        float ripple = sin(15.0 * r) * 0.03;
                        fishPos += normalize(fishPos) * ripple;

                        gl_Position = vec4(fishPos, a_Pos.z, 1.0);
                    }
                    """,

                    """
                    #version 330 core

                    layout(location = 0) out vec4 color;

                    in vec4 vert_Color;

                    void main(){
                        float gray = dot(vert_Color.rgb, vec3(0.299, 0.4, 0.114));
                        color = vec4(vec3(gray), vert_Color.a);
                    }
                    """);

                var shader2 = Renderer.GenShader("""
                    #version 330 core

                    layout(location = 0) in vec2 a_Pos;

                    out vec2 vert_Pos;

                    void main(){
                        vert_Pos = a_Pos * 0.5 + 0.5; //[-1; 1] -> [0; 1]
                        vec2 pos = a_Pos * 0.5;
                        gl_Position = vec4(pos.x + 0.5, pos.y + 0.55, 0.0, 1.0);
                    }
                    """,
                    """
                    #version 330 core

                    layout(location = 0) out vec4 color;

                    in vec2 vert_Pos;

                    void main(){
                        vec2 center = vec2(0.5, 0.5);
                        vec2 pos = vert_Pos - center;
                        float dist = length(pos);

                        if(dist <= 0.11){
                            color = vec4(0.1, 0.1, 0.1, 1.0);
                            return;
                        }

                        //iris
                        if(dist <= 0.27){
                            float angle = atan(pos.y, pos.x); //[-pi; pi]
                            float r = dist;

                            float flames = sin(20.0 * r + 10.0 * angle);
                            flames = pow(flames, 0.8);

                            vec3 col = mix(vec3(0.0), vec3(1.0, 0.0, 0.0), flames);
                            col = mix(col, vec3(1.0, 0.5, 0.0), flames * 0.7);
                            col = mix(col, vec3(1.0, 1.0, 0.0), flames * 0.4);

                            col += vec3(1.0, 0.8, 0.5) * (0.05/r);

                            color = vec4(clamp(col, 0.0, 1.0), 1.0);
                            return;
                        }

                        if(dist <= 0.28){
                            color = vec4(183.0/255.0, 95.0/255.0, 24.0/255.0, 0.97);
                            return;
                        }

                        //sclera
                        color = vec4(0.05, 0.05, 0.05, 1.0);
                    }
                    """
                    );

                var square = new ChainLink(square_vbo, square_ibo, fish_eye_shader);
                var triangle = new ChainLink(triangle_vbo, triangle_ibo, fish_eye_shader);
                var hex = new ChainLink(hex_vbo, hex_ibo, shader2);

                RenderChain.PushLink(square);
                RenderChain.PushLink(triangle);
                RenderChain.PushLink(hex);

                Log.Warn(RenderChain.GetChainLinks().Count);
                RenderChain.Compress();
                Log.Warn(RenderChain.GetChainLinks().Count);
                RenderChain.Bake();
            }

            Log.Info("Hello, Triangle!");
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
                Renderer.Clear(0.3f, 0.5f, 0.78f, 1);
                Renderer.Render();
            }

            dtStopwatch.Stop();

            RenderChain.Dispose();
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
