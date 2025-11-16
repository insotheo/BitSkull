using BitSkull;
using BitSkull.Core;
using BitSkull.Events;

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
        }
    }
}
