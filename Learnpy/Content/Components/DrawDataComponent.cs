using Microsoft.Xna.Framework;

namespace Learnpy.Content.Components
{
    public struct DrawDataComponent
    {
        public float Depth;
        public Vector2 Origin;
        public Vector2 Size;
        public Color Tint;

        public DrawDataComponent(Vector2 orig, Vector2 size, float depth = 1f, Color tint = default)
        {
            Origin = orig;
            Size = size;
            Depth = depth;
            Tint = tint;
            if (Tint == default)
                Tint = Color.White;
        }
    }
}
