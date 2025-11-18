using BitSkull.Numerics;
using System.Collections.Generic;

namespace BitSkull.InputSystem
{
    public static class Input
    {
        private static readonly HashSet<KeyCode> _keysDown = new();
        private static readonly HashSet<KeyCode> _keysUp = new();
        private static readonly HashSet<MouseButton> _mouseBtnDown = new();
        private static readonly HashSet<MouseButton> _mouseBtnUp = new();
        private static Vec2D _mousePos = new();
        private static Vec2D _mouseOffset = new();

        public static bool IsKeyDown(KeyCode key) => _keysDown.Contains(key);
        public static bool IsAnyKeyDown() => _keysDown.Count > 0;
        public static bool IsKeyUp(KeyCode key) => _keysUp.Contains(key);
        public static bool IsAnyKeyUp() => _keysUp.Count > 0;

        public static bool IsMouseButtonDown(MouseButton button) => _mouseBtnDown.Contains(button);
        public static bool IsAnyMouseButtonDown() => _mouseBtnDown.Count > 0;
        public static bool IsMouseButtonUp(MouseButton button) => _mouseBtnUp.Contains(button);
        public static bool IsAnyMouseButtonUp() => _mouseBtnUp.Count > 0;
        public static Vec2D GetMousePos() => _mousePos;
        public static Vec2D GetMouseWheelOffset() => _mouseOffset;


        internal static void Update()
        {
            _keysUp.Clear();
            _mouseBtnUp.Clear();

            _mouseOffset.X = 0f;
            _mouseOffset.Y = 0f;
        }

        internal static void OnKeyDown(KeyCode key)
        {
            _keysDown.Add(key);
            _keysUp.Remove(key);
        }
        internal static void OnKeyUp(KeyCode key)
        {
            _keysDown.Remove(key);
            _keysUp.Add(key);
        }
        internal static void OnMouseDown(MouseButton button)
        {
            _mouseBtnDown.Add(button);
            _mouseBtnUp.Remove(button);
        }
        internal static void OnMouseUp(MouseButton button)
        {
            _mouseBtnDown.Remove(button);
            _mouseBtnUp.Add(button);
        }
        internal static void OnMouseMove(Vec2D pos) => _mousePos = pos;
        internal static void OnMouseScroll(Vec2D offset) => _mouseOffset = offset;

    }
}
