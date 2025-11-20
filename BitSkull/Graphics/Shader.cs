using BitSkull.Numerics;
using System;
using System.Numerics;

namespace BitSkull.Graphics
{
    public abstract class Shader : IDisposable
    {
        private static int _nextID = 0;
        public int ID { get; private set; }

        public bool IsValid { get; protected set; } = false;
        public VertexShaderInfo VertexShaderInfo { get; private set; }

        protected Shader(VertexShaderInfo vertexShaderInfo)
        {
            ID = _nextID++;

            if (String.IsNullOrEmpty(vertexShaderInfo.ModelUniformName) ||
                String.IsNullOrEmpty(vertexShaderInfo.ViewUniformName) ||
                String.IsNullOrEmpty(vertexShaderInfo.ProjectionUniformName))
                throw new ArgumentException("VertexShaderInfo contains null or empty uniform names");
            VertexShaderInfo = vertexShaderInfo;

        }
        protected abstract void ApplyShaderInfo();

        public virtual void Use() { }
        public virtual void ZeroUse() { }
        public virtual void Dispose() { }

        #region Uniforms

        public virtual void SetUniform(string name, int value) { }
        public virtual void SetUniform(string name, float value) { }
        public virtual void SetUniform(string name, double value) { }
        public virtual void SetUniform(string name, Vec2D value) { }
        public virtual void SetUniform(string name, Vec3D value) { }
        public virtual void SetUniform(string name, Color4 value) { }
        public virtual void SetUniform(string name, Matrix4x4 value) { }
        public virtual void SetUniform(string name, Texture2D texture, int slot = 0) { }

        #endregion
    }
}
