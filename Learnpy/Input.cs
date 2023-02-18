using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using static Learnpy.Collision;

namespace Learnpy
{
    public class Input
    {
        static MouseState _oldMBState;
        static Vector2 _oldPos;

        static KeyboardState _oldKBState;

        public static Vector2 MousePos {
            get {
                return new Vector2(Mouse.GetState().X, Mouse.GetState().Y) / (GameOptions.Size / EntryPoint.Instance.Size);
            }
        }

        public static AABB MouseBox => new AABB(MousePos, Vector2.One);

        public static Vector2 PositionDifference
        {
            get
            {
                if (_oldPos != default)
                    return MousePos - _oldPos;

                return Vector2.Zero;
            }
        }

        public static void Update()
        {
            _oldPos = MousePos;
            _oldMBState = Mouse.GetState();
            _oldKBState = Keyboard.GetState();
        }

        public static bool PressedKey(Keys key) => Keyboard.GetState().IsKeyDown(key) && _oldKBState.IsKeyUp(key);

        public static bool HeldKey(Keys key) => Keyboard.GetState().IsKeyDown(key);

        public static bool ReleasedKey(Keys key) => Keyboard.GetState().IsKeyUp(key) && _oldKBState.IsKeyDown(key);

        public static bool LMBClicked => Mouse.GetState().LeftButton == ButtonState.Pressed && _oldMBState.LeftButton == ButtonState.Released;

        public static bool LMBReleased => Mouse.GetState().LeftButton == ButtonState.Released && _oldMBState.LeftButton == ButtonState.Pressed;

        public static bool LMBHeld => Mouse.GetState().LeftButton == ButtonState.Pressed;
    }
}
