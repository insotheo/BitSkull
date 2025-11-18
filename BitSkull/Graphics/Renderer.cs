using BitSkull.Graphics.Queue;
using System;
using System.Collections.Generic;

namespace BitSkull.Graphics
{
    public class Renderer : IDisposable
    {
        public RendererApi API { get; private set; }
        public IRenderBackend Context { get; private set; }
        private bool _initialized = false;

        internal RenderQueue Queue { get; private set; }
        //TODO: Make Queue private and add methods to add Renderables to it

        public Renderer(RendererApi api, IRenderBackend ctx)
        {
            API = api;
            Context = ctx;
            _initialized = Context != null;
            if (_initialized)
            {
                Context.Configure();
                Queue = new RenderQueue();
            }
        }

        ////////////////////////////

        public void ResizeFramebuffer(int x, int y) => Context?.ResizeFramebuffer(x, y);

        public void Clear() => Context?.Clear();
        public void Clear(float r, float g, float b, float a) => Context?.Clear(r, g, b, a);

        #region Generators

        public VertexBuffer GenVertexBuffer(float[] vertices)
        {
#if DEFAULT_PLATFORM
            if (API == RendererApi.OpenGL) return new Platform.OpenGL.OpenGLVertexBuffer(vertices);
#endif
            return null;
        }

        public IndexBuffer GenIndexBuffer(uint[] indices)
        {
#if DEFAULT_PLATFORM
            if (API == RendererApi.OpenGL) return new Platform.OpenGL.OpenGLIndexBuffer(indices);
#endif
            return null;
        }

        /// <summary>
        /// For OpenGL vertexShader and fragmentShader should be sources
        /// </summary>
        public Shader GenShader(string vertexShader, string fragmentShader)
        {
#if DEFAULT_PLATFORM
            if (API == RendererApi.OpenGL) return new Platform.OpenGL.OpenGLShader(vertexShader, fragmentShader);
#endif
            return null;
        }

        #endregion

        public void ExecuteRenderQueue()
        {
            if (!_initialized || !Queue.Initialized) return;

            foreach ((Shader shader, List<Renderable> links) in Queue.Queue)
                Context.Draw(shader, links);
        }

        ////////////////////////////


        public void Dispose()
        {
            if (_initialized)
            {
                Context.Dispose();
            }
        }
    }
}
