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
                Application sandboxApp = new Application("Sandbox");
                //Application newApp = new("ABC"); //ERROR
                sandboxApp.Run();
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
    