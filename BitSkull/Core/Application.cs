using System;

namespace BitSkull.Core
{
    public class Application
    {
        private static Application _inst;
        private string _name;
        public bool IsRunning { get; private set; } = false;

        public Application(string appName)
        {
            if (_inst != null) throw new Exception("Application is already created!");

            _name = appName;
            _inst = this;
        }

        public static Application GetAppInstance() => _inst;

        public void Run()
        {
            IsRunning = true;
            while (IsRunning) ;
            IsRunning = false;
        }
    }
}
