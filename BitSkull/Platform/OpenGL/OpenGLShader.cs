using BitSkull.Graphics;
using Silk.NET.OpenGL;
using System;

namespace BitSkull.Platform.OpenGL
{
    internal sealed class OpenGLShader : Graphics.Shader
    {
        private uint _program;

        internal OpenGLShader(string vertexSrc, string fragmentSrc)
        {
            GL gl = (Renderer.Context as GlRendererContext).Gl;

            uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertexShader, vertexSrc);
            gl.CompileShader(vertexShader);

            string infoLog = gl.GetShaderInfoLog(vertexShader);
            if (!String.IsNullOrEmpty(infoLog))
            {
                gl.DeleteShader(vertexShader);
                Log.Error($"Error compiling vertex shader: {infoLog}");
                return;
            }

            uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragmentShader, fragmentSrc);
            gl.CompileShader(fragmentShader);

            infoLog = gl.GetShaderInfoLog(fragmentShader);
            if (!String.IsNullOrEmpty(infoLog))
            {
                gl.DeleteShader(vertexShader);
                gl.DeleteShader(fragmentShader);
                Log.Error($"Error compiling fragment shader: {infoLog}");
                return;
            }

            _program = gl.CreateProgram();
            gl.AttachShader(_program, vertexShader);
            gl.AttachShader(_program, fragmentShader);
            gl.LinkProgram(_program);

            gl.GetProgram(_program, GLEnum.LinkStatus, out int status);
            if(status == 0)
            {
                Log.Error($"Error linking shader: {gl.GetProgramInfoLog(_program)}");
                gl.DeleteProgram(_program);
                gl.DeleteShader(vertexShader);
                gl.DeleteShader(fragmentShader);
                return;
            }

            gl.DetachShader(_program, vertexShader);
            gl.DetachShader(_program, fragmentShader);
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);
        }

        public override void Use() => (Renderer.Context as GlRendererContext).Gl.UseProgram(_program);
        public override void ZeroUse() => (Renderer.Context as GlRendererContext).Gl.UseProgram(0);

        public override void Dispose() => (Renderer.Context as GlRendererContext).Gl.DeleteProgram(_program);
    }
}
