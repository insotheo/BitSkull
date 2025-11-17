using BitSkull.Core;
using BitSkull.InputSystem;

namespace Sandbox
{
    internal class TestLayer : Layer
    {
        public override void OnUpdate(float dt)
        {
            if (Input.IsKeyUp(KeyCode.Escape))
                Application.GetAppInstance().Stop();
        }
    }
}
