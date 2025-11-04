using BitSkull;
using BitSkull.Core;
using BitSkull.Events;
using BitSkull.InputSystem;

namespace Sandbox
{
    internal class TestLayer : Layer
    {
        int counter = 0;

        public override void OnEvent(Event e)
        {
            if(e is WindowResizeEvent resizeEvent)
            {
                Log.Info($"{resizeEvent.Width} x {resizeEvent.Height}");
            }
        }

        public override void OnUpdate(float dt)
        {
            if (Input.IsKeyUp(KeyCode.Escape))
            {
                Application.GetAppInstance().Stop();
            }
            else if (Input.IsAnyKeyUp() || Input.IsAnyMouseButtonUp())
            {
                counter++;
                Log.Info($"FPS: {(int)(1 / dt)} | Counter: {counter}");
            }
        }
    }
}
