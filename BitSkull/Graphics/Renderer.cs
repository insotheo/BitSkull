using BitSkull.Graphics.Queue;
using System.Collections.Generic;

namespace BitSkull.Graphics
{
    internal static class Renderer
    {
        public static RendererApi API { get; private set; }
        public static IRenderBackend Context { get; private set; }
        private static bool _initialized = false;

        public static void Init(RendererApi api, IRenderBackend ctx)
        {
            API = api;
            Context = ctx;
            _initialized = Context != null;
            if (_initialized)
            {
                Context.Configure();
                RenderQueue.Initialize();
            }
        }

        ////////////////////////////

        public static void ResizeFramebuffer(int x, int y) => Context?.ResizeFramebuffer(x, y);

        public static void Clear() => Context?.Clear();
        public static void Clear(float r, float g, float b, float a) => Context?.Clear(r, g, b, a);

        #region Generators

        public static VertexBuffer GenVertexBuffer(float[] vertices)
        {
#if DEFAULT_PLATFORM
            if (API == RendererApi.OpenGL) return new Platform.OpenGL.OpenGLVertexBuffer(vertices);
#endif
            return null;
        }

        public static IndexBuffer GenIndexBuffer(uint[] indices)
        {
#if DEFAULT_PLATFORM
            if (API == RendererApi.OpenGL) return new Platform.OpenGL.OpenGLIndexBuffer(indices);
#endif
            return null;
        }

        /// <summary>
        /// For OpenGL vertexShader and fragmentShader should be sources
        /// </summary>
        public static Shader GenShader(string vertexShader, string fragmentShader)
        {
#if DEFAULT_PLATFORM
            if (API == RendererApi.OpenGL) return new Platform.OpenGL.OpenGLShader(vertexShader, fragmentShader);
#endif
            return null;
        }

        #endregion

        public static void ExecuteRenderQueue()
        {
            if (!_initialized || !RenderQueue.Initialized) return;

            foreach ((Shader shader, List<Renderable> links) in RenderQueue.Queue)
                Context.Draw(shader, links);
        }

        ////////////////////////////


        public static void Dispose()
        {
            if (_initialized)
            {
                Context.Dispose();
            }
        }
    }
}
