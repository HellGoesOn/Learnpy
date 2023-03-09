using Microsoft.Xna.Framework;

namespace Learnpy.Content.Components
{
    public struct DrawDataComponent
    {
        public float Depth;
        public Vector2 Origin;
        public Vector2 Scale;
        public Color Tint;

        public DrawDataComponent(Vector2 orig, Vector2 scale, float depth = 1f, Color tint = default)
        {
            Origin = orig;
            Scale = scale;
            Depth = depth;
            Tint = tint;
            if (Tint == default)
                Tint = Color.White;
        }
    }
}
