using BitSkull.Graphics.Queue;
using System;
using System.Collections.Generic;

namespace BitSkull.Graphics
{
    public class Renderer : IDisposable
    {
        public RendererApi API { get; private set; }
        public IRenderBackend Backend { get; private set; }
        private bool _initialized = false;

        private RenderQueue Queue;

        public Renderer(RendererApi api, IRenderBackend backend)
        {
            API = api;
            Backend = backend;
            _initialized = Backend != null;
            if (_initialized)
            {
                Backend.Configure();
            }
        }

        ////////////////////////////

        public void ResizeFramebuffer(int x, int y) => Backend?.ResizeFramebuffer(x, y);

        public void Clear() => Backend?.Clear();
        public void Clear(float r, float g, float b, float a) => Backend?.Clear(r, g, b, a);

        #region Generators

        public VertexBuffer GenVertexBuffer(float[] vertices)
        {
            if(Backend != null) return Backend.GenVertexBuffer(vertices);
            return null;
        }

        public IndexBuffer GenIndexBuffer(uint[] indices)
        {
            if(Backend != null) return Backend.GenIndexBuffer(indices);
            return null;
        }

        /// <summary>
        /// For OpenGL vertexShader and fragmentShader should be sources
        /// </summary>
        public Shader GenShader(string vertexShader, string fragmentShader)
        {
            if(Backend != null) return Backend.GenShader(vertexShader, fragmentShader);
            return null;
        }

        #endregion

        public void ExecuteRenderQueue()
        {
            if (!VerifyInitialization()) return;

            foreach ((Shader shader, List<Renderable> links) in Queue.GetQueue())
                Backend.Draw(shader, links);
        }

        ////////////////////////////

        #region Queue

        public void InitializeRenderQueue()
        {
            if (!_initialized) return;
            Queue = new RenderQueue();
        }

        public void PushToRenderQueue(Renderable item)
        {
            if (!VerifyInitialization()) return;
            Queue.Push(item);
        }

        public void PopFromRenderQueue(Renderable item)
        {
            if (!VerifyInitialization()) return;
            Queue.Pop(item);
        }

        public void BakeRenderQueue()
        {
            if (!VerifyInitialization()) return;
            Queue.Bake();
        }

        public void DisposeRenderQueue()
        {
            if (!VerifyInitialization()) return;
            Queue.Dispose();
        }

        #endregion


        public void Dispose()
        {
            if (_initialized)
                Backend.Dispose();
        }

        private bool VerifyInitialization() => _initialized && (Queue == null ? false : Queue.Initialized);
    }
}
