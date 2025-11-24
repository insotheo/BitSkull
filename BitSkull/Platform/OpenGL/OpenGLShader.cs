using BitSkull.Graphics;
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

        private readonly GL _gl;

        internal OpenGLShader(GL gl, string vertexSrc, string fragmentSrc, BufferLayout layout, VertexShaderInfo vertexShaderInfo)
            : base(layout, vertexShaderInfo)
        {
            _gl = gl;

            uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
            _gl.ShaderSource(vertexShader, vertexSrc);
            _gl.CompileShader(vertexShader);

            string infoLog = _gl.GetShaderInfoLog(vertexShader);
            if (!String.IsNullOrEmpty(infoLog))
            {
                _gl.DeleteShader(vertexShader);
                Log.Error($"Error compiling vertex shader: {infoLog}");
                return;
            }

            uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(fragmentShader, fragmentSrc);
            _gl.CompileShader(fragmentShader);

            infoLog = _gl.GetShaderInfoLog(fragmentShader);
            if (!String.IsNullOrEmpty(infoLog))
            {
                _gl.DeleteShader(vertexShader);
                _gl.DeleteShader(fragmentShader);
                Log.Error($"Error compiling fragment shader: {infoLog}");
                return;
            }

            _program = _gl.CreateProgram();
            _gl.AttachShader(_program, vertexShader);
            _gl.AttachShader(_program, fragmentShader);
            _gl.LinkProgram(_program);

            _gl.GetProgram(_program, GLEnum.LinkStatus, out int status);
            if (status == 0)
            {
                Log.Error($"Error linking shader: {_gl.GetProgramInfoLog(_program)}");
                _gl.DeleteProgram(_program);
                _gl.DeleteShader(vertexShader);
                _gl.DeleteShader(fragmentShader);
                return;
            }

            _gl.DetachShader(_program, vertexShader);
            _gl.DetachShader(_program, fragmentShader);
            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);

            _uniforms = new Dictionary<string, int>();
            ApplyShaderInfo();
            IsValid = true;
        }

        public override void Use() => _gl.UseProgram(_program);
        public override void ZeroUse() => _gl.UseProgram(0);

        public override void Dispose() => _gl.DeleteProgram(_program);

        #region uniforms
        private int GetUniform(string name)
        {
            if (_uniforms.ContainsKey(name))
                return _uniforms[name];

            int loc = _gl.GetUniformLocation(_program, name);
            if (loc == -1)
                Log.Warn($"Uniform '{name}' not found in shader!");
            _uniforms.Add(name, loc);
            return loc;
        }


        public override void SetUniform(string name, int value)
        {
            int loc = GetUniform(name);
            if (loc == -1) return;
            _gl.Uniform1(GetUniform(name), value);
        }
        public override void SetUniform(string name, float value)
        {
            int loc = GetUniform(name);
            if (loc == -1) return;
            _gl.Uniform1(GetUniform(name), value);
        }
        public override void SetUniform(string name, double value)
        {
            int loc = GetUniform(name);
            if (loc == -1) return;
            _gl.Uniform1(GetUniform(name), value);
        }
        public override void SetUniform(string name, Vec2D value)
        {
            int loc = GetUniform(name);
            if (loc == -1) return;
            _gl.Uniform2(GetUniform(name), value.X, value.Y);
        }
        public override void SetUniform(string name, Vec3D value)
        {
            int loc = GetUniform(name);
            if (loc == -1) return;
            _gl.Uniform3(GetUniform(name), value.X, value.Y, value.Z);
        }
        public override void SetUniform(string name, Color4 value)
        {
            int loc = GetUniform(name);
            if (loc == -1) return;
            _gl.Uniform4(GetUniform(name), value.R, value.G, value.B, value.A);
        }
        public unsafe override void SetUniform(string name, Matrix4x4 value)
        {
            int loc = GetUniform(name);
            if (loc == -1) return;
            _gl.UniformMatrix4(loc, 1, false, (float*)&value);
        }
        public override void SetUniform(string name, Texture2D texture, int slot = 0)
        {
            int loc = GetUniform(name);
            if (loc == -1) return;
            OpenGLTexture2D glTexture = texture as OpenGLTexture2D;
            _gl.ActiveTexture(GLEnum.Texture0 + slot);
            glTexture.Bind();
            _gl.Uniform1(loc, slot);
        }

        #endregion

        protected override void ApplyShaderInfo()
        {
            if (GetUniform(VertexShaderInfo.ModelUniformName) == -1)
                Log.Warn($"Shader compilation warn: required Model uniform('{VertexShaderInfo.ModelUniformName}') not found in shader");
            if (GetUniform(VertexShaderInfo.ViewUniformName) == -1)
                Log.Warn($"Shader compilation warn: required View uniform('{VertexShaderInfo.ViewUniformName}') not found in shader");
            if (GetUniform(VertexShaderInfo.ProjectionUniformName) == -1)
                Log.Warn($"Shader compilation warn: required Projection uniform('{VertexShaderInfo.ProjectionUniformName}') not found in shader");
        }

    }
}
