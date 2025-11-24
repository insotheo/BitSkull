namespace BitSkull.Numerics
{
    public struct Color3
    {
        public float R, G, B;

        public Color3(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Color3()
        {
            R = G = B = 0f;
        }

        public override string ToString() => $"(R: {R}, G: {G}, B: {B})";
        public override int GetHashCode() => R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        public override bool Equals(object obj)
        {
            if (obj is Color3 o) return o.R == R && o.G == G && o.B == B;
            return false;
        }
    }
}