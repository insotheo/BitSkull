using BitSkull.Numerics;
using System.Numerics;

namespace BitSkull.Graphics
{
    public interface IUniformValue
    {
        void Apply(Shader shader, string uniformName);
        void SetValue(object value);
    }

    public class UniformInt : IUniformValue
    {
        public int Value { get; set; }

        public UniformInt(int value) => Value = value;

        public void Apply(Shader shader, string uniformName) => shader.SetUniform(uniformName, Value);
        public void SetValue(object value) => Value = (int)value;
    }

    public class UniformFloat : IUniformValue
    {
        public float Value { get; set; }

        public UniformFloat(float value) => Value = value;

        public void Apply(Shader shader, string uniformName) => shader.SetUniform(uniformName, Value);
        public void SetValue(object value) => Value = (float)value;
    }

    public class UniformDouble : IUniformValue
    {
        public double Value { get; set; }
        public UniformDouble(double value) => Value = value;

        public void Apply(Shader shader, string uniformName) => shader.SetUniform(uniformName, Value);
        public void SetValue(object value) => Value = (double)value;
    }

    public class UniformVec2D : IUniformValue
    {
        public Vec2D Value { get; set; }

        public UniformVec2D(Vec2D value) => Value = value;

        public void Apply(Shader shader, string uniformName) => shader.SetUniform(uniformName, Value);
        public void SetValue(object value) => Value = (Vec2D)value;
    }

    public class UniformVec3D : IUniformValue
    {
        public Vec3D Value { get; set; }

        public UniformVec3D(Vec3D value) => Value = value;

        public void Apply(Shader shader, string uniformName) => shader.SetUniform(uniformName, Value);
        public void SetValue(object value) => Value = (Vec3D)value;
    }

    public class UniformColor3 : IUniformValue
    {
        public Color3 Value { get; set; }

        public UniformColor3(Color3 value) => Value = value;

        public void Apply(Shader shader, string uniformName) => shader.SetUniform(uniformName, Value);
        public void SetValue(object value) => Value = (Color3)value;
    }

    public class UniformColor4 : IUniformValue
    {
        public Color4 Value { get; set; }

        public UniformColor4(Color4 value) => Value = value;

        public void Apply(Shader shader, string uniformName) => shader.SetUniform(uniformName, Value);
        public void SetValue(object value) => Value = (Color4)value;
    }

    public class UniformMat4 : IUniformValue
    {
        public Matrix4x4 Value { get; set; }

        public UniformMat4(Matrix4x4 value) => Value = value;

        public void Apply(Shader shader, string uniformName) => shader.SetUniform(uniformName, Value);
        public void SetValue(object value) => Value = (Matrix4x4)value;
    }

    public class UniformTexture2D : IUniformValue
    {
        public Texture2D Value { get; set; }
        public int Slot { get; set; } = 0;

        public UniformTexture2D(Texture2D value) => Value = value;

        public void Apply(Shader shader, string uniformName) => shader.SetUniform(uniformName, Value);
        public void SetValue(object value) => Value = (Texture2D)value;
    }
}