using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Learnpy
{
    public static class Util
    {
        public static void DrawRectangle(SpriteBatch sb, Vector2 position, Vector2 size, Color clr = default, float depth = 0f)
        {
            sb.Draw(Assets.GetTexture("Pixel"), position, new Rectangle(0, 0, (int)size.X, (int)size.Y), clr, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
        }
    }
}
