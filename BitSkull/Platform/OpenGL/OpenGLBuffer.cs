using BitSkull.Core;
using BitSkull.Graphics;
using Silk.NET.OpenGL;
using System;

namespace BitSkull.Platform.OpenGL
{
    internal sealed unsafe class OpenGLVertexBuffer : VertexBuffer
    {
        private uint _data;
        private BufferLayout _layout;

        public OpenGLVertexBuffer(float[] vertices)
        {
            GL gl = (Application.GetAppRenderer().Context as OpenGLBackend).Gl;
            _data = gl.GenBuffer();

            Bind();
            fixed (void* v = vertices)
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
            Unbind();
        }

        public override void SetLayout(BufferLayout layot) => _layout = layot;
        public override BufferLayout GetLayot() => _layout;
        public unsafe override void BindLayout()
        {
            GL gl = (Application.GetAppRenderer().Context as OpenGLBackend).Gl;

            Bind();
            uint idx = 0;
            foreach (BufferElement el in _layout)
            {
                gl.EnableVertexAttribArray(idx);
                gl.VertexAttribPointer(
                    idx,
                    (int)el.GetComponentCount(),
                    el.Type switch
                    {
                        ShaderDataType.Int => GLEnum.Int,
                        ShaderDataType.Int2 => GLEnum.Int,
                        ShaderDataType.Int3 => GLEnum.Int,
                        ShaderDataType.Int4 => GLEnum.Int,

                        ShaderDataType.Float => GLEnum.Float,
                        ShaderDataType.Float2 => GLEnum.Float,
                        ShaderDataType.Float3 => GLEnum.Float,
                        ShaderDataType.Float4 => GLEnum.Float,

                        ShaderDataType.Mat3 => GLEnum.FloatMat3,
                        ShaderDataType.Mat4 => GLEnum.FloatMat4,

                        ShaderDataType.Bool => GLEnum.Bool,

                        _ => throw new Exception("Unknown shader data type!")
                    },
                    el.Normalized,
                    _layout.Stride,
                    (void*)el.Offset
                );
                idx += 1;
            }
        }

        public override void Bind() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.BindBuffer(BufferTargetARB.ArrayBuffer, _data);
        public override void Unbind() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        public override void Dispose() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.DeleteBuffer(_data);
    }

    internal sealed unsafe class OpenGLIndexBuffer : IndexBuffer
    {
        private uint _data;
        private uint _count;

        public OpenGLIndexBuffer(uint[] indices)
        {
            GL gl = (Application.GetAppRenderer().Context as OpenGLBackend).Gl;
            _count = (uint)indices.Length;

            _data = gl.GenBuffer();

            Bind();
            fixed (void* i = indices)
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(_count * sizeof(uint)), i, BufferUsageARB.StaticDraw);
            Unbind();
        }

        public override uint GetCount() => _count;

        public override void Bind() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _data);
        public override void Unbind() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        public override void Dispose() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.DeleteBuffer(_data);
    }
}
