using System;
using System.Windows.Input;

namespace SimpleGameEngine.Core
{
    public class InputManager
    {
        private static InputManager _instance;
        public static InputManager Instance => _instance ?? (_instance = new InputManager());
        private InputManager(){}
        

        public static bool KeyUp(Key key)
        {
            return Keyboard.IsKeyUp(key);
        }
        
        public static bool KeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }
        
        public static bool MouseUp(MouseButton btn)
        {
            switch (btn)
            {
                case MouseButton.Left:
                    return Mouse.LeftButton == MouseButtonState.Released;
                case MouseButton.Middle:
                    return Mouse.MiddleButton == MouseButtonState.Released;
                case MouseButton.Right:
                    return Mouse.RightButton == MouseButtonState.Released;
                default:
                    throw new ArgumentOutOfRangeException(nameof(btn), btn, null);
            }
        }
        
        public static bool MouseDown(MouseButton btn)
        {
            switch (btn)
            {
                case MouseButton.Left:
                    return Mouse.LeftButton == MouseButtonState.Pressed;
                case MouseButton.Middle:
                    return Mouse.MiddleButton == MouseButtonState.Pressed;
                case MouseButton.Right:
                    return Mouse.RightButton == MouseButtonState.Pressed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(btn), btn, null);
            }
        }
    }
}