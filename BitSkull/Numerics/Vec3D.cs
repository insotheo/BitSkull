using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace BitSkull.Numerics
{
    public struct Vec3D
    {
        public float X, Y, Z;

        public Vec3D(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        public Vec3D()
        {
            X = 0f;
            Y = 0f;
            Z = 0f;
        }

        public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z);

        public Vec3D Normalized()
        {
            float len = Length;
            if (len == 0f) return new();
            return new(X / len, Y / len, Z / len);
        }

        public static Vec3D operator +(Vec3D a, Vec3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vec3D operator *(Vec3D a, float c) => new(a.X * c, a.Y * c, a.Z * c);
        public static Vec3D operator *(float c, Vec3D a) => new(a.X * c, a.Y * c, a.Z * c);


        public Vector3 ToSystem() => new(X, Y, Z);


        public override string ToString() => $"({X}, {Y}, {Z})";
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj is Vec3D o) return o.X == X && o.Y == Y && o.Z == Z;
            return false;
        }
    }
}
