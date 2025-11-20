using BitSkull.Assets;
using BitSkull.Graphics;
using BitSkull.Numerics;
using Silk.NET.OpenGL;
using System.Numerics;

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
            //_gl.DepthFunc(GLEnum.Less);
            _gl.Enable(GLEnum.CullFace);
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

        public unsafe void Draw(RenderQueue queue)
        {
            Graphics.Shader prevShader = null;
            Material prevMat = null;
            Mesh prevMesh = null;

            foreach (Renderable r in queue)
            {
                if (!r.IsValid)
                    continue;

                if (r.Material.Shader != prevShader)
                {
                    r.Material.Shader.Use();
                    prevShader = r.Material.Shader;
                }

                if (prevMat != r.Material)
                {
                    r.Material.Apply();
                    prevMat = r.Material;
                }

                if (prevMesh != r.Mesh)
                {
                    r.Mesh.UsePlatform();
                    prevMesh = r.Mesh;
                }

                prevShader.SetUniform(prevShader.VertexShaderInfo.ModelUniformName, r.Transform.GetTransformMatrix());
                _gl.DrawElements(PrimitiveType.Triangles, r.Mesh.GetIndexCount(), DrawElementsType.UnsignedInt, null);
            }

            _gl.BindVertexArray(0);
            _gl.UseProgram(0);
        }

        public VertexBuffer GenVertexBuffer(float[] vertices) => new OpenGLVertexBuffer(_gl, vertices);
        public IndexBuffer GenIndexBuffer(uint[] indices) => new OpenGLIndexBuffer(_gl, indices);
        public Graphics.Shader GenShader(string vertexShader, string fragmentShader, VertexShaderInfo vertexShaderInfo) => new OpenGLShader(_gl, vertexShader, fragmentShader, vertexShaderInfo);
        public Texture2D GenTexture2D(Image image) => new OpenGLTexture2D(_gl, image);
    }
}
