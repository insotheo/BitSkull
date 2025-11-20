using System.Numerics;

namespace BitSkull.Numerics
{
    public sealed class Transform3D
    {
        public Vec3D Position;
        public Vec3D Rotation;
        public Vec3D Scale;

        public Transform3D()
        {
            Position = Vec3D.Zero;
            Rotation = Vec3D.Zero;
            Scale = new Vec3D(1f, 1f, 1f);
        }

        public Matrix4x4 GetTransformMatrix()
        {
            Matrix4x4 translation = Matrix4x4.CreateTranslation(Position.X, Position.Y, Position.Z);
            Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Matrix4x4 scale = Matrix4x4.CreateScale(Scale.X, Scale.Y, Scale.Z);
            return scale * rotation * translation;
        }
    }
}
