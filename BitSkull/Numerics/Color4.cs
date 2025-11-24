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

        public override string ToString() => $"(R: {R}, G: {G}, B: {B}, A: {A})";
        public override int GetHashCode() => R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
        public override bool Equals(object obj)
        {
            if (obj is Color4 o) return o.R == R && o.G == G && o.B == B && o.A == A;
            return false;
        }
    }
}
