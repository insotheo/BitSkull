namespace BitSkull.Graphics
{
    internal static class Renderer
    {
        public static RendererApi API { get; private set; }
        public static IRendererContext Context { get; private set; }
        private static bool _initialized = false;

        public static void Init(RendererApi api, IRendererContext ctx)
        {
            API = api;
            Context = ctx;
            _initialized = Context != null;
            if (_initialized) Context.Configure();
        }

        ////////////////////////////

        public static void ResizeFramebuffer(int x, int y) => Context?.ResizeFramebuffer(x, y);

        public static void Clear() => Context?.Clear();
        public static void Clear(float r, float g, float b, float a) => Context?.Clear(r, g, b, a);

        public static VertexBuffer GenVertexBuffer(float[] vertices)
        {
#if DEFAULT_PLATFORM
            if (API == RendererApi.OpenGL) return new Platform.OpenGL.OpenGLVertexBuffer(vertices);
#endif
            return null;
        }

        public static IndexBuffer GenIndexBuffer(uint[] indices)
        {
#if DEFAULT_PLATFORM
            if (API == RendererApi.OpenGL) return new Platform.OpenGL.OpenGLIndexBuffer(indices);
#endif
            return null;
        }

        ////////////////////////////


        public static void Dispose()
        {
            if (_initialized)
            {
                Context.Dispose();
            }
        }
    }
}
