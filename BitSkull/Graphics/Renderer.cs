namespace BitSkull.Graphics
{
    internal static class Renderer
    {
        private static RendererApi _api;
        private static IRendererContext _ctx;
        private static bool _initialized = false;

        public static void Init(RendererApi api, IRendererContext ctx)
        {
            _api = api;
            _ctx = ctx;
            _initialized = _ctx != null;
            if (_initialized) _ctx.Configure();
        }

        ////////////////////////////

        public static void ResizeFramebuffer(int x, int y) => _ctx?.ResizeFramebuffer(x, y);

        public static void Clear() => _ctx?.Clear();
        public static void Clear(float r, float g, float b, float a) => _ctx?.Clear(r, g, b, a);

        ////////////////////////////


        public static void Dispose()
        {
            if (_initialized)
            {
                _ctx.Dispose();
            }
        }
    }
}
