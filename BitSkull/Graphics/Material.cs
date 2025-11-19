using BitSkull.Numerics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace BitSkull.Graphics
{
    public class Material
    {
        private Shader _shader;
        private Dictionary<string, Action<Shader>> _pendingUniforms;
        private Dictionary<string, Action<Shader, int>> _pendingTextureUniforms;

        private Dictionary<string, Texture2D> _textures;

        internal Material(Shader shader)
        {
            _shader = shader;
            _pendingUniforms = new Dictionary<string, Action<Shader>>();
            _pendingTextureUniforms = new Dictionary<string, Action<Shader, int>>();
            _textures = new Dictionary<string, Texture2D>();
        }

        #region Setters

        public void SetInt(string name, int value) => _pendingUniforms[name] = shader => shader.SetUniform(name, value);
        public void SetReal(string name, float value) => _pendingUniforms[name] = shader => shader.SetUniform(name, value);
        public void SetReal(string name, double value) => _pendingUniforms[name] = shader => shader.SetUniform(name, value);
        public void SetVec2D(string name, Vec2D value) => _pendingUniforms[name] = shader => shader.SetUniform(name, value);
        public void SetVec3D(string name, Vec3D value) => _pendingUniforms[name] = shader => shader.SetUniform(name, value);
        public void SetColor(string name, Color4 value) => _pendingUniforms[name] = shader => shader.SetUniform(name, value);
        public void SetMat4(string name, Matrix4x4 value) => _pendingUniforms[name] = shader => shader.SetUniform(name, value);

        public void SetTexure(string name, Texture2D value) => _pendingTextureUniforms[name] = (shader, idx) => shader.SetUniform(name, value);
        public void SetTexure(string name, string textureName) => _pendingTextureUniforms[name] = (shader, idx) => shader.SetUniform(name, _textures[textureName]);

        #endregion

        internal void SaveTextureReference(string name, Texture2D texture) => _textures.Add(name, texture);
        internal void DisposeTexture(string name)
        {
            if (!_textures.ContainsKey(name)) return;
            _textures[name].Dispose();
            _textures.Remove(name);
        }
        internal void RemoveTextureReference(string name)
        {
            if (!_textures.ContainsKey(name)) return;
            _textures.Remove(name);
        }

        internal int GetPendingTexturesCount() => _pendingTextureUniforms.Count;

        internal void Apply()
        {
            int i = 0;
            foreach (var action in _pendingTextureUniforms.Values)
            {
                action.Invoke(_shader, i);
                i += 1;
            }

            foreach (var action in _pendingUniforms.Values)
                action(_shader);
        }

        internal void ClearPending()
        {
            _pendingUniforms.Clear();
            _pendingTextureUniforms.Clear();
        }
    }
}
