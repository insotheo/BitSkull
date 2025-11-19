using BitSkull.Assets;
using BitSkull.Graphics;
using Silk.NET.OpenGL;

namespace BitSkull.Platform.OpenGL
{
    internal class OpenGLTexture2D : Texture2D
    {
        private uint _texture;

        private readonly GL _gl;

        public unsafe OpenGLTexture2D(GL gl, Image img)
        {
            _gl = gl;

            _texture = _gl.GenTexture();
            _gl.ActiveTexture(GLEnum.Texture0);
            Bind();

            fixed(byte* ptr = img.GetData())
                _gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgba, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

            _gl.TextureParameter(_texture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            _gl.TextureParameter(_texture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            _gl.TextureParameter(_texture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            _gl.TextureParameter(_texture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //mipmaps
            _gl.GenerateMipmap(GLEnum.Texture2D);

            Unbind();
        }

        internal uint GetHandle() => _texture;

        public override void Bind() => _gl.BindTexture(GLEnum.Texture2D, _texture);
        public override void Unbind() => _gl.BindTexture(GLEnum.Texture2D, 0);

        public override void Dispose() => _gl.DeleteTexture(_texture);
    }
}
