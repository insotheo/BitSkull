using BitSkull.Numerics;

namespace BitSkull.Graphics
{
    public class Renderable
    {
        public long SortKey { get; private set; }

        internal Mesh Mesh { get; private set; }
        internal Material Material { get; private set; }
        internal Transform3D Transform { get; private set; }

        internal bool IsValid => Mesh != null && Mesh.IsValid && Material != null && Material?.Shader != null;

        public Renderable(Mesh mesh, Material material)
        {
            Mesh = mesh;
            Material = material;
            Transform = new Transform3D();
            ComputeSortKey();
        }

        private void ComputeSortKey() => SortKey = (long)Material.Shader.ID << 25 | (long)Material.ID << 15 | (long)Mesh.ID; //recompute if any changes;
    }
}
