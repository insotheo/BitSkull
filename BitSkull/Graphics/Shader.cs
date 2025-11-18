using BitSkull.Numerics;
using System;
using System.Numerics;

namespace BitSkull.Graphics
{
    public class Shader : IDisposable
    {
        protected Shader() { }

        public virtual void Use() { }
        public virtual void ZeroUse() { }
        public virtual void Dispose() { }


        //uniforms
        public virtual void SetUniform(string name, int value) { }
        public virtual void SetUniform(string name, float value) { }
        public virtual void SetUniform(string name, double value) { }
        public virtual void SetUniform(string name, Vec2D value) { }
        public virtual void SetUniform(string name, Vec3D value) { }
        public virtual void SetUniform(string name, Color4 value) { }
        public virtual void SetUniform(string name, Matrix4x4 value) { }
    }
}
