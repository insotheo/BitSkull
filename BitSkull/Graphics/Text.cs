using BitSkull.Assets;
using System;

namespace BitSkull.Graphics
{
    public sealed class Text
    {
        public Renderable Renderable { get; private set; }
        private Renderer _renderer;
        private bool _shaderVertexPositionHasZ;
        private BufferLayout _layout;

        public Font Font { get; private set; }
        public string Content { get; private set; }
        private float _nativeScale;

        public Text(Renderer renderer, BufferLayout layout, Renderable renderable, Font font, string content = "", float nativeScale = 5f, bool shaderVertexPositionHasZ = true)
        {
            _renderer = renderer;
            _layout = layout;
            _shaderVertexPositionHasZ = shaderVertexPositionHasZ;
            Renderable = renderable;
            if (Renderable == null)
                throw new ArgumentNullException("Renderable is null");

            Font = font;
            Content = content;
            _nativeScale = nativeScale;

            UpdateGeometry();
        }
        public void SetContent(string content)
        {
            Content = content;
            UpdateGeometry();
        }

        public void SetNativeScale(float value)
        {
            _nativeScale = value;
            UpdateGeometry();
        }

        private void UpdateGeometry()
        {
            var txtData = Font.GetTextGeometry(Content, _nativeScale);
            float[] vertices = new float[txtData.glypthVertices.Count * (_shaderVertexPositionHasZ ? 5 : 4)];
            int idx = 0;
            foreach(GlypthVertex vertex in txtData.glypthVertices)
            {
                vertices[idx++] = vertex.Pos.X;
                vertices[idx++] = vertex.Pos.Y;
                if(_shaderVertexPositionHasZ)
                    vertices[idx++] = 0f;

                vertices[idx++] = vertex.UV.X;
                vertices[idx++] = vertex.UV.Y;
            }

            VertexBuffer vbo = _renderer.GenVertexBuffer(vertices);
            vbo.SetLayout(_layout);
            IndexBuffer ibo = _renderer.GenIndexBuffer(txtData.indices);
            Renderable.Mesh.Recreate(vbo, ibo, _renderer);
        }
    }
}
