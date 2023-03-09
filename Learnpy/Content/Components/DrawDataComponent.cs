using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Learnpy.Content.Components
{
    public struct DrawDataComponent
    {
        public float Depth;
        public Vector2 Origin;
        public Vector2 Scale;
        public Color Tint;
        public SpriteEffects SpriteEffects {get; set; }

        public DrawDataComponent(Vector2 orig, Vector2 scale, float depth = 1f, Color tint = default)
        {
            Origin = orig;
            Scale = scale;
            Depth = depth;
            Tint = tint;
            SpriteEffects = SpriteEffects.None;
            if (Tint == default)
                Tint = Color.White;
        }
    }
}
