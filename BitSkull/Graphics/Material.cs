using BitSkull.Numerics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace BitSkull.Graphics
{
    public class Material
    {
        private Shader _shader;
        private List<Action<Shader>> _pendingUniforms;

        internal Material(Shader shader)
        {
            _shader = shader;
            _pendingUniforms = new List<Action<Shader>>();
        }

        #region Setters
        public void SetInt(string name, int value) => _pendingUniforms.Add(shader => shader.SetUniform(name, value));
        public void SetReal(string name, float value) => _pendingUniforms.Add(shader => shader.SetUniform(name, value));
        public void SetReal(string name, double value) => _pendingUniforms.Add(shader => shader.SetUniform(name, value));
        public void SetVec2D(string name, Vec2D value) => _pendingUniforms.Add(shader => shader.SetUniform(name, value));
        public void SetVec3D(string name, Vec3D value) => _pendingUniforms.Add(shader => shader.SetUniform(name, value));
        public void SetColor(string name, Color4 value) => _pendingUniforms.Add(shader => shader.SetUniform(name, value));
        public void SetMat4(string name, Matrix4x4 value) => _pendingUniforms.Add(shader => shader.SetUniform(name, value));
        #endregion

        internal void Apply()
        {
            foreach(var action in _pendingUniforms)
                action(_shader);
        }
    }
}
