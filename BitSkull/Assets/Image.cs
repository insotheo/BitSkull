using StbImageSharp;
using System;
using System.IO;

namespace BitSkull.Assets
{
    public sealed class Image : IDisposable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private byte[] _data;

        private bool _disposed = false;

        public Image(Stream stream)
        {
            ImageResult res = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            Width = res.Width;
            Height = res.Height;
            _data = res.Data;
            _disposed = false;
        }

        public byte[] GetData() => _data;

        public void Dispose()
        {
            if (_disposed) return;
            _data = null;
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
