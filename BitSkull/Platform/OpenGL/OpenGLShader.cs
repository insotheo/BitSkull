using BitSkull.Core;
using BitSkull.Numerics;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace BitSkull.Platform.OpenGL
{
    internal sealed class OpenGLShader : Graphics.Shader
    {
        private Dictionary<string, int> _uniforms;

        private uint _program;

        internal OpenGLShader(string vertexSrc, string fragmentSrc)
        {
            GL gl = (Application.GetAppRenderer().Context as OpenGLBackend).Gl;

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
            if (status == 0)
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

            _uniforms = new Dictionary<string, int>();
        }

        public override void Use() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.UseProgram(_program);
        public override void ZeroUse() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.UseProgram(0);

        public override void Dispose() => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.DeleteProgram(_program);

        #region uniforms
        private int GetUniform(string name)
        {
            if (_uniforms.ContainsKey(name))
                return _uniforms[name];

            int loc = (Application.GetAppRenderer().Context as OpenGLBackend).Gl.GetUniformLocation(_program, name);
            _uniforms.Add(name, loc);
            return loc;
        }


        public override void SetUniform(string name, int value) => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.Uniform1(GetUniform(name), value);
        public override void SetUniform(string name, float value) => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.Uniform1(GetUniform(name), value);
        public override void SetUniform(string name, double value) => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.Uniform1(GetUniform(name), value);
        public override void SetUniform(string name, Vec2D value) => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.Uniform2(GetUniform(name), value.X, value.Y);
        public override void SetUniform(string name, Vec3D value) => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.Uniform3(GetUniform(name), value.X, value.Y, value.Z);
        public override void SetUniform(string name, Color4 value) => (Application.GetAppRenderer().Context as OpenGLBackend).Gl.Uniform4(GetUniform(name), value.R, value.G, value.B, value.A);
        public unsafe override void SetUniform(string name, Matrix4x4 value)
        {
            int loc = GetUniform(name);
            (Application.GetAppRenderer().Context as OpenGLBackend).Gl.UniformMatrix4(loc, 1, false, (float*)&value);
        }

        #endregion
    }
}
