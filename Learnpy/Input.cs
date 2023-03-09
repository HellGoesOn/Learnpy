using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using static Learnpy.Collision;

namespace Learnpy
{
    public struct MouseResult
    {
        public Vector2 Position;
        public AABB Bounds;

        public MouseResult(Vector2 position, AABB bounds)
        {
            Position = position;
            Bounds = bounds;
        }
    }

    public partial class Input
    {
        static MouseState _oldMBState;
        static Vector2 _oldPos;

        static KeyboardState _oldKBState;

        public static Vector2 ScaledMousePos {
            get {
                return new Vector2(Mouse.GetState().X, Mouse.GetState().Y) / (GameOptions.Size / EntryPoint.Instance.Size);
            }
        }

        public static Vector2 MousePos => new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

        public static MouseResult ScreenToWorldSpace(World world)
        {
            Vector2 campos = world.camera.Position;
            Vector2 v = ((ScaledMousePos + campos )/ world.camera.zoom) ;
            return new(v, new AABB(v, Vector2.One));
        }

        public static AABB MouseBox => new AABB(ScaledMousePos, Vector2.One);

        public static Vector2 PositionDifference
        {
            get
            {
                if (_oldPos != default)
                    return ScaledMousePos - _oldPos;

                return Vector2.Zero;
            }
        }

        public static void Update()
        {
            _oldPos = ScaledMousePos;
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
