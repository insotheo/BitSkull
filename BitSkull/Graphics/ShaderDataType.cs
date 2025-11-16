using System;

namespace BitSkull.Graphics
{
    public enum ShaderDataType
    {
        None = 0,
        Int, Int2, Int3, Int4,
        Float, Float2, Float3, Float4,
        Mat3, Mat4,
        Bool
    }

    public static class ShaderDataTypeTools
    {
        public static uint ShaderDataTypeSize(ShaderDataType type)
        {
            return type switch
            {
                ShaderDataType.Float => 4,
                ShaderDataType.Float2 => 4 * 2,
                ShaderDataType.Float3 => 4 * 3,
                ShaderDataType.Float4 => 4 * 4,

                ShaderDataType.Int => 4,
                ShaderDataType.Int2 => 4 * 2,
                ShaderDataType.Int3 => 4 * 3,
                ShaderDataType.Int4 => 4 * 4,

                ShaderDataType.Mat3 => 4 * 3 * 3,
                ShaderDataType.Mat4 => 4 * 4 * 4,

                ShaderDataType.Bool => 1,

                _ => throw new Exception("Unknwon shader data type")
            };
        }
    }
}
