using System;
using BitSkull;
using BitSkull.Core;

namespace Sandbox
{
    internal class Program
    {
        static void Main()
        {
            Log.Pattern = "[%T]: %E";
            try
            {
                using (Application sandboxApp = new Application("Sandbox"))
                {
                    sandboxApp.CreateWindow(800, 600);
                    sandboxApp.Run();
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
    