using BitSkull.Numerics;

namespace BitSkull.Graphics
{
    public class Renderable
    {
        public long SortKey { get; private set; }

        private Mesh _mesh;
        public Mesh Mesh
        {
            get => _mesh;
            set { _mesh = value; ComputeSortKey(); }
        }

        private Material _material;
        public Material Material
        {
            get => _material;
            set { _material = value; ComputeSortKey(); }
        }

        public Transform3D Transform { get; private set; }

        internal bool IsValid => Mesh != null && Mesh.IsValid && Material != null && Material?.Shader != null;

        public Renderable(Mesh mesh, Material material)
        {
            _mesh = mesh;
            _material = material;
            Transform = new Transform3D();

            ComputeSortKey();
        }

        private void ComputeSortKey() => SortKey = (long)Material.Shader.ID << 25 | (long)Material.ID << 15 | (long)Mesh.ID;
    }
}
