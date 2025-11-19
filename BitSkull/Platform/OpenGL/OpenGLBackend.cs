using BitSkull.Assets;
using BitSkull.Graphics;
using BitSkull.Graphics.Queue;
using Silk.NET.OpenGL;
using System.Collections.Generic;

namespace BitSkull.Platform.OpenGL
{
    internal sealed class OpenGLBackend : IRenderBackend
    {
        private GL _gl;

        public void Initialize(Silk.NET.Core.Contexts.INativeContext ctx) => _gl = GL.GetApi(ctx);
        public void Dispose() => _gl.CurrentVTable.Dispose();

        public void Configure()
        {
            //_gl.Enable(GLEnum.DepthTest);
            //_gl.Enable(GLEnum.CullFace);
            //_gl.DepthFunc(GLEnum.Less);
            _gl.Enable(GLEnum.Blend);
            _gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        }

        public void Clear() => _gl.Clear(ClearBufferMask.ColorBufferBit);

        public void Clear(float r, float g, float b, float a)
        {
            _gl.ClearColor(r, g, b, a);
            Clear();
        }

        public void ResizeFramebuffer(int x, int y) => _gl.Viewport(0, 0, (uint)x, (uint)y);

        public IPlatformRenderable CreatePlatformRenderable(VertexBuffer vertexBuffer, IndexBuffer indexBuffer) => new OpenGLRenderable(_gl, vertexBuffer, indexBuffer);

        public unsafe void Draw(Graphics.Shader shader, List<Renderable> links)
        {
            shader.Use();

            int texuturesCount = -1;
            foreach (Renderable link in links)
            {
                if(link.Material.GetPendingTexturesCount() > texuturesCount)
                    texuturesCount = link.Material.GetPendingTexturesCount();

                link.Material.Apply();
                link.Platform.Bind();
                _gl.DrawElements(GLEnum.Triangles, link.IBuffer.GetCount(), GLEnum.UnsignedInt, null);
            }

            shader.ZeroUse();

            for (int i = 0; i <= texuturesCount; i++)
            {
                _gl.ActiveTexture(GLEnum.Texture0 + i);
                _gl.BindTexture(GLEnum.Texture2D, 0);
            }
            _gl.BindVertexArray(0);
        }

        public VertexBuffer GenVertexBuffer(float[] vertices) => new OpenGLVertexBuffer(_gl, vertices);
        public IndexBuffer GenIndexBuffer(uint[] indices) => new OpenGLIndexBuffer(_gl, indices);
        public Graphics.Shader GenShader(string vertexShader, string fragmentShader) => new OpenGLShader(_gl, vertexShader, fragmentShader);
        public Texture2D GenTexture2D(Image image) => new OpenGLTexture2D(_gl, image);
    }
}
