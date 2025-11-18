using System;
using System.Diagnostics.CodeAnalysis;

namespace BitSkull.Numerics
{
    public struct Vec2D
    {
        public float X, Y;

        public Vec2D(float x, float y)
        {
            X = x; Y = y;
        }

        public Vec2D()
        {
            X = 0f;
            Y = 0f;
        }

        public float Length => MathF.Sqrt(X * X + Y * Y);
        public Vec2D Normalized()
        {
            float len = Length;
            if (len == 0f) return new();
            return new(X / len, Y / len);
        }

        public static Vec2D operator +(Vec2D a, Vec2D b) => new(a.X + b.X, a.Y + b.Y);
        public static Vec2D operator *(Vec2D a, float c) => new(a.X * c, a.Y * c);
        public static Vec2D operator *(float c, Vec2D a) => new(a.X * c, a.Y * c);


        public override string ToString() => $"({X}, {Y})";
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if(obj is Vec2D o) return o.X == X && o.Y == Y;
            return false;
        }
    }
}
