using BitSkull.Graphics;
using BitSkull.Numerics;
using System.Collections.Generic;
using System.IO;
using static StbTrueTypeSharp.StbTrueType;

namespace BitSkull.Assets
{
    public struct GlypthVertex
    {
        public Vec2D Pos;
        public Vec2D UV;

        public GlypthVertex(Vec2D pos, Vec2D uv)
        {
            Pos = pos;
            UV = uv;
        }
    }

    public sealed class Font
    {
        public const int FirstChar = 32;
        public const int CharCount = 96;
        public const int AtlasSize = 512;

        public byte[] Atlas { get; private set; }
        public stbtt_bakedchar[] Glyphs { get; private set; }
        public Texture2D FontTexture { get; private set;  }

        public Font(Stream fontStream, int pxHeight = 32)
        {
            byte[] fontData;
            using(MemoryStream ms = new MemoryStream())
            {
                fontStream.CopyTo(ms);
                fontStream.Seek(0, SeekOrigin.Begin);
                fontData = ms.ToArray();
            }

            Atlas = new byte[AtlasSize * AtlasSize];
            Glyphs = new stbtt_bakedchar[CharCount];

            unsafe
            {
                fixed(byte* font_p = fontData)
                {
                    fixed(byte* atlass_p = Atlas)
                    {
                        fixed(stbtt_bakedchar* gliphs_p = Glyphs)
                        {
                            int res = stbtt_BakeFontBitmap(font_p, 0, pxHeight, atlass_p, AtlasSize, AtlasSize, FirstChar, CharCount, gliphs_p);
                            if (res <= 0)
                                Log.Error("Failed to load font!");
                        }
                    }
                }
            }
        }

        public (List<GlypthVertex> glypthVertices, uint[] indices) GetTextGeometry(string text, float scale = 5f)
        {
            List<GlypthVertex> verts = new List<GlypthVertex>();
            List<uint> indices = new List<uint>();

            float x = 0f, y = 0f;

            foreach (char ch in text)
            {
                char c = ch;
                if (c < FirstChar || c >= FirstChar + CharCount)
                    c = '?';

                var g = Glyphs[c - FirstChar];

                //glyph quad
                float x0 = x + g.xoff;
                float y0 = y;
                float x1 = x0 + (g.x1 - g.x0);
                float y1 = y0 + (g.y1 - g.y0);

                //UVs
                float s0 = g.x0 / (float)AtlasSize;
                float t0 = g.y0 / (float)AtlasSize;
                float s1 = g.x1 / (float)AtlasSize;
                float t1 = g.y1 / (float)AtlasSize;

                uint baseIndex = (uint)verts.Count;

                verts.Add(new GlypthVertex(new Vec2D(x0 / scale, y0 / scale), new Vec2D(s0, t1))); //0
                verts.Add(new GlypthVertex(new Vec2D(x1 / scale, y0 / scale), new Vec2D(s1, t1))); //1
                verts.Add(new GlypthVertex(new Vec2D(x1 / scale, y1 / scale), new Vec2D(s1, t0))); //2
                verts.Add(new GlypthVertex(new Vec2D(x0 / scale, y1 / scale), new Vec2D(s0, t0))); //3

                indices.Add(baseIndex + 0);
                indices.Add(baseIndex + 1);
                indices.Add(baseIndex + 2);
                indices.Add(baseIndex + 0);
                indices.Add(baseIndex + 2);
                indices.Add(baseIndex + 3);

                x += g.xadvance;
            }

            return (verts, indices.ToArray());
        }

        public float MeasureText(string text)
        {
            float width = 0f;
            foreach(char c in text)
            {
                if (c < FirstChar || c >= FirstChar + CharCount)
                    continue;

                width += Glyphs[c - FirstChar].xadvance;
            }
            return width;
        }

        internal void SetFontTexture(Texture2D texture) => FontTexture = texture;
    }
}
