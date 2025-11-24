using BitSkull.Assets;
using BitSkull.Numerics;

namespace BitSkull.Graphics
{
    public sealed class Text
    {
        public Renderable Renderable { get; private set; }
        private Renderer _renderer;
        private Shader _shader;
        private bool _shaderVertexPositionHasZ;

        public Font Font { get; private set; }
        public Transform3D Transform => Renderable.Transform;

        public string Content { get; private set; }
        private float _nativeScale;

        internal Text(Renderer renderer, Shader shader, Font font, string content = "", float nativeScale = 5f, bool shaderVertexPositionHasZ = true)
        {
            _renderer = renderer;
            _shaderVertexPositionHasZ = shaderVertexPositionHasZ;
            _shader = shader;

            Renderable = new Renderable(_renderer.CreateMesh(null, null), new Material(_shader));

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
            foreach (GlypthVertex vertex in txtData.glypthVertices)
            {
                vertices[idx++] = vertex.Pos.X;
                vertices[idx++] = vertex.Pos.Y;
                if (_shaderVertexPositionHasZ)
                    vertices[idx++] = 0f;

                vertices[idx++] = vertex.UV.X;
                vertices[idx++] = vertex.UV.Y;
            }

            VertexBuffer vbo = _renderer.GenVertexBuffer(vertices);
            vbo.SetLayout(_shader.Layout);
            IndexBuffer ibo = _renderer.GenIndexBuffer(txtData.indices);
            Renderable.Mesh.Recreate(vbo, ibo, _renderer);
        }
    }
}
