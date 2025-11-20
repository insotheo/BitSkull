using System;

namespace BitSkull.Numerics
{
    public static class Maths
    {
        public static T Clamp<T>(T value, T max, T min) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            else if (value.CompareTo(max) > 0) return max;
            return value;
        }

        public static float DegToRad(float degrees) => degrees * MathF.PI / 180f;
        public static float RadToDeg(float radians) => radians * 180f / MathF.PI;

        public static float DotProduct(Vec2D a, Vec2D b) => a.X * b.X + a.Y * b.Y;
        public static float DotProduct(Vec3D a, Vec3D b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static Vec3D CrossProduct(Vec3D a, Vec3D b) => new Vec3D(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );
    }
}
