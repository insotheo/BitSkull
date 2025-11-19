using BitSkull.Assets;
using System;
using System.Collections.Generic;

namespace BitSkull.Graphics
{
    public class Renderer : IDisposable
    {
        public RendererApi API { get; private set; }
        public IRenderBackend Backend { get; private set; }
        private bool _initialized = false;

        private RenderQueue _queue;
        private Dictionary<string, Shader> _shaders;

        public Renderer(RendererApi api, IRenderBackend backend)
        {
            API = api;
            Backend = backend;
            _initialized = Backend != null;
            if (_initialized)
            {
                Backend.Configure();
                _queue = new RenderQueue();
                _shaders = new Dictionary<string, Shader>();
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

        public Texture2D GenTexure2D(Image img)
        {
            if(Backend != null) return Backend.GenTexture2D(img);
            return null;
        }

        #endregion

        public void ExecuteRenderQueue()
        {
            if (!_initialized) return;
            Backend.Draw(_queue);
        }

        ////////////////////////////

        #region Shader Management

        /// <summary>
        /// For OpenGL vertexShader and fragmentShader should be sources
        /// </summary>
        public void CreateShader(string shaderName, string vertexShader, string fragmentShader)
        {
            if (Backend == null) return;
            _shaders.Add(shaderName, Backend.GenShader(_shaders.Count, vertexShader, fragmentShader));
        }

        public Shader GetShader(string shaderName)
        {
            if (!_initialized || !_shaders.ContainsKey(shaderName)) return null;
            return _shaders[shaderName];
        }

        #endregion


        #region Queue

        public void InitializeRenderQueue()
        {
            if (!_initialized) return;
            _queue = new RenderQueue();
        }

        public void PushToRenderQueue(string shaderName, Renderable item)
        {
            if (!_initialized) return;
            _queue.PushRenderable(item);
        }

        public void PopFromRenderQueue(Renderable item)
        {
            if (!_initialized) return;
            _queue.PopRenderable(item);
        }

        public void BakeRenderQueue()
        {
            if (!_initialized) return;
            _queue.BakeAll(this);
        }
        #endregion


        public void DisposeRenderQueue()
        {
            if(!_initialized) return;
            _queue.DisposeAndClear();
        }

        public void Dispose()
        {
            if (_initialized)
            {
                foreach (Shader shader in _shaders.Values)
                    shader.Dispose();
                _shaders.Clear();

                Backend.Dispose();
            }
        }
    }
}
