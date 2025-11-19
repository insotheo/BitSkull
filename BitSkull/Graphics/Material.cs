using BitSkull.Numerics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace BitSkull.Graphics
{
    public class Material
    {
        private static int _nextID = 0;
        public int ID { get; private set; }

        public Shader Shader { get; private set; }

        private Dictionary<string, IUniformValue> _uniforms;
        private List<string> _changedUniforms;

        private Dictionary<string, Texture2D> _textures;

        public Material(Shader shader)
        {
            ID = _nextID++;
            Shader = shader;
            _uniforms = new Dictionary<string, IUniformValue>();
            _textures = new Dictionary<string, Texture2D>();
            _changedUniforms = new List<string>();
        }

        #region Setters
        private void SetUniform(string name, object value)
        {
            if (!_uniforms.ContainsKey(name))
                _uniforms.Add(name, value switch
                {
                    int => new UniformInt((int)value),
                    float => new UniformFloat((float)value),
                    double => new UniformDouble((double)value),
                    Vec2D => new UniformVec2D((Vec2D)value),
                    Vec3D => new UniformVec3D((Vec3D)value),
                    Color4 => new UniformColor4((Color4)value),
                    Matrix4x4 => new UniformMat4((Matrix4x4)value),
                    Texture2D => new UniformTexture2D((Texture2D)value),

                    _ => throw new ArgumentException($"Unsupported uniform type: {value.GetType()}")
                });

            _uniforms[name].SetValue(value);
            _changedUniforms.Add(name);
        }

        public void SetInt(string name, int value) => SetUniform(name, value);
        public void SetReal(string name, float value) => SetUniform(name, value);
        public void SetReal(string name, double value) => SetUniform(name, value);
        public void SetVec2D(string name, Vec2D value) => SetUniform(name, value);
        public void SetVec3D(string name, Vec3D value) => SetUniform(name, value);
        public void SetColor(string name, Color4 value) => SetUniform(name, value);
        public void SetMat4(string name, Matrix4x4 value) => SetUniform(name, value);

        public void SetTexture(string name, Texture2D value) => SetUniform(name, value);
        public void SetTexture(string name, string textureName) => SetUniform(name, _textures[textureName]);

        #endregion

        #region Textures

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

        #endregion

        internal void Apply()
        {
            int texturesCount = 0;
            foreach (string uniformName in _changedUniforms)
            {
                if (_uniforms[uniformName] is UniformTexture2D uTexture)
                {
                    uTexture.Slot = texturesCount;
                    uTexture.Apply(Shader, uniformName);
                    texturesCount += 1;
                }
                else
                    _uniforms[uniformName].Apply(Shader, uniformName);
            }
            _changedUniforms.Clear();
        }
    }
}
