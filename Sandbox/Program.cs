using BitSkull;
using BitSkull.Core;
using BitSkull.Graphics;
using System;

namespace Sandbox
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Log.Pattern = "[%T]: %E";
            try
            {
                using (Application sandboxApp = new Application("Sandbox"))
                {
                    sandboxApp.CreateWindow(800, 550, api: RendererApi.OpenGL);
                    sandboxApp.PushLayer(new TestLayer());
                    sandboxApp.Run();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
