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

        private bool _frameActive = false;

        private List<RenderQueue> _queues;

        //GPU resources
        private Dictionary<string, Shader> _shaders;
        private List<Texture2D> _textures;
        private List<Mesh> _meshes;

        public Renderer(RendererApi api, IRenderBackend backend)
        {
            API = api;
            Backend = backend;
            _initialized = Backend != null;
            if (_initialized)
            {
                Backend.Configure();

                _queues = new List<RenderQueue>();

                _shaders = new Dictionary<string, Shader>();
                _textures = new List<Texture2D>();
                _meshes = new List<Mesh>();
            }
        }

        ////////////////////////////

        public void ResizeFramebuffer(int x, int y) => Backend?.ResizeFramebuffer(x, y);

        public void Clear() => Backend?.Clear();
        public void Clear(float r, float g, float b, float a) => Backend?.Clear(r, g, b, a);

        ////////////////////////////

        #region Mesh
        
        public VertexBuffer GenVertexBuffer(float[] vertices)
        {
            if (Backend != null) return Backend.GenVertexBuffer(vertices);
            return null;
        }

        public IndexBuffer GenIndexBuffer(uint[] indices)
        {
            if (Backend != null) return Backend.GenIndexBuffer(indices);
            return null;
        }

        public Mesh CreateMesh(VertexBuffer vbo, IndexBuffer ibo)
        {
            Mesh mesh = new Mesh(vbo, ibo, this);
            _meshes.Add(mesh);
            return mesh;
        }

        #endregion

        #region Textures

        public Texture2D GenTexture2D(Image img)
        {
            if (Backend == null) return null;
            Texture2D texture = Backend.GenTexture2D(img);
            _textures.Add(texture);
            return texture;
        }

        #endregion

        #region Shader Management

        /// <summary>
        /// For OpenGL vertexShader and fragmentShader should be sources
        /// </summary>
        public void CreateShader(string shaderName, string vertexShader, string fragmentShader, VertexShaderInfo vertexShaderInfo)
        {
            if (Backend == null) return;
            Shader shader = Backend.GenShader(vertexShader, fragmentShader, vertexShaderInfo);
            if (!shader.IsValid) return;
            if (_shaders.ContainsKey(shaderName))
            {
                Log.Warn($"Shader '{shaderName}' already exists - replacing", "Shader Manager");
                _shaders[shaderName].Dispose();
                _shaders.Remove(shaderName);
            }
            _shaders.Add(shaderName, shader);
        }

        public Shader GetShader(string shaderName)
        {
            if (!_initialized || !_shaders.ContainsKey(shaderName)) return null;
            return _shaders[shaderName];
        }

        #endregion


        #region Queue

        public void BeginFrame()
        {
            if(!_initialized) return;
            if (_frameActive)
            {
                Log.Error("Frame is already activated", "Renderer");
                return;
            }
            _queues.Clear();
            _frameActive = true;
        }

        public RenderQueue CreateQueue()
        {
            if (!_initialized) return null;
            if (!_frameActive)
            {
                Log.Error("Cannot create queue: frame is not activated", "Renderer");
                return null;
            }
            RenderQueue queue = new RenderQueue();
            _queues.Add(queue);
            return queue;
        }

        public void EndFrame()
        {
            if(!_initialized) return;
            if (!_frameActive)
            {
                Log.Error("EndFrame called wihtout matching BeginFrame");
                return;
            }
            if (_queues.Count > 0)
            {
                Clear(0.3f, 0.4f, 0.7f, 1f); //TODO: take data from the first camera
                foreach (RenderQueue queue in _queues)
                {
                    queue.Sort();
                    Backend.Draw(queue);
                }
                _queues.Clear();
            }
            else
                Clear(0.15f, 0.15f, 0.15f, 1f);
            _frameActive = false;
        }

        #endregion

        public void Dispose()
        {
            if (_initialized)
            {
                foreach (RenderQueue queue in _queues)
                    queue.Dispose();
                _queues.Clear();

                foreach (Mesh mesh in _meshes)
                    mesh.Dispose();
                _meshes.Clear();

                foreach (Texture2D texture in _textures)
                    texture.Dispose();
                _textures.Clear();

                foreach (Shader shader in _shaders.Values)
                    shader.Dispose();
                _shaders.Clear();

                Backend.Dispose();
            }
        }
    }
}
