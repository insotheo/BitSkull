using BitSkull.Graphics;
using Silk.NET.OpenGL;

namespace BitSkull.Platform.OpenGL
{
    internal sealed unsafe class OpenGLVertexBuffer : VertexBuffer
    {
        private uint _data;

        public OpenGLVertexBuffer(float[] vertices)
        {
            GL gl = (Renderer.Context as GlRendererContext).Gl;
            _data = gl.GenBuffer();

            Bind();
            fixed (void* v = vertices)
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
            Unbind();
        }

        public override void Bind() => (Renderer.Context as GlRendererContext).Gl.BindBuffer(BufferTargetARB.ArrayBuffer, _data);
        public override void Unbind() => (Renderer.Context as GlRendererContext).Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        public override void Dispose() => (Renderer.Context as GlRendererContext).Gl.DeleteBuffer(_data);
    }

    internal sealed unsafe class OpenGLIndexBuffer : IndexBuffer
    {
        private uint _data;
        private uint _count;

        public OpenGLIndexBuffer(uint[] indices)
        {
            GL gl = (Renderer.Context as GlRendererContext).Gl;
            _count = (uint)indices.Length;

            _data = gl.GenBuffer();

            Bind();
            fixed (void* i = indices)
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(_count * sizeof(uint)), i, BufferUsageARB.StaticDraw);
            Unbind();
        }

        public override uint GetCount() => _count;

        public override void Bind() => (Renderer.Context as GlRendererContext).Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _data);
        public override void Unbind() => (Renderer.Context as GlRendererContext).Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        public override void Dispose() => (Renderer.Context as GlRendererContext).Gl.DeleteBuffer(_data);
    }
}
