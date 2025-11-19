using StbImageSharp;
using System.IO;

namespace BitSkull.Assets
{
    public sealed class Image
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private byte[] _data;

        public Image(Stream stream)
        {
            ImageResult res = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            Width = res.Width;
            Height = res.Height;
            _data = res.Data;
        }

        public byte[] GetData() => _data;
    }
}
