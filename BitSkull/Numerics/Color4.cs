using System;

namespace BitSkull.Numerics
{
    public struct Color4
    {
        public float R, G, B, A;

        public Color4(float r, float g, float b, float a = 1f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color4()
        {
            R = G = B = 0f;
            A = 1f;
        }

        public Color4(string hex, float a = 1f)
        {
            if (String.IsNullOrEmpty(hex))
                throw new ArgumentException("Hex string cannot be null or empty");

            hex = hex.TrimStart('#');
            if (hex.Length != 6)
                throw new ArgumentException("Hex string must be 6(RRGGBB) characters long");

            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 4), 16);
            byte b = Convert.ToByte(hex.Substring(4, 6), 16);

            R = r / 255f;
            G = g / 255f;
            B = b / 255f;
        }

        public override string ToString() => $"(R: {R}, G: {G}, B: {B}, A: {A})";
        public override int GetHashCode() => R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
        public override bool Equals(object obj)
        {
            if (obj is Color4 o) return o.R == R && o.G == G && o.B == B && o.A == A;
            return false;
        }
    }
}
