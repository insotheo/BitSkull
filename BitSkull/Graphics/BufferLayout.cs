using System;
using System.Collections;

namespace BitSkull.Graphics
{
    public sealed class BufferElement
    {
        public string Name { get; private set; }
        public ShaderDataType Type { get; private set; }
        public uint Size { get; private set; }
        public uint Offset { get; internal set; }
        public bool Normalized { get; private set; }

        public BufferElement(string name, ShaderDataType type, bool normalized = false)
        {
            Name = name;
            Type = type;
            Size = ShaderDataTypeTools.ShaderDataTypeSize(type);
            Offset = 0;
            Normalized = normalized;
        }

        public uint GetComponentCount()
        {
            return Type switch
            {
                ShaderDataType.Float => 1,
                ShaderDataType.Float2 => 2,
                ShaderDataType.Float3 => 3,
                ShaderDataType.Float4 => 4,

                ShaderDataType.Int => 1,
                ShaderDataType.Int2 => 2,
                ShaderDataType.Int3 => 3,
                ShaderDataType.Int4 => 4,

                ShaderDataType.Mat3 => 3 * 3,
                ShaderDataType.Mat4 => 4 * 4,

                ShaderDataType.Bool => 1,

                _ => throw new Exception("Unknown shader data type!")
            };
        }
    }
    

    public sealed class BufferLayout : IEnumerable
    {
        private BufferElement[] _elements;
        public uint Stride { get; private set; }

        public BufferLayout(params BufferElement[] elements)
        {
            _elements = elements;
            CalculateOffsetsAndStride();
        }

        private void CalculateOffsetsAndStride()
        {
            uint offset = 0;
            Stride = 0;
            foreach(BufferElement el in _elements)
            {
                el.Offset = offset;
                offset += el.Size;
                Stride += el.Size;
            }
        }

        public IEnumerator GetEnumerator() => _elements.GetEnumerator();
    }
}
