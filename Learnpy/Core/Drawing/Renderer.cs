using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Learnpy.Core.Drawing
{
    public static class Renderer
    {
        static SpriteBatch batch => EntryPoint.Instance.spriteBatch;

        public static RenderTarget2D Target;

        public static void DrawText(string text, Vector2 position, SpriteFont font, Color color, float rotation, Vector2 scale, Vector2 origin, SpriteEffects spriteEffects)
        {
            batch.DrawString(font, text, position, color, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects sfx)
        {
            batch.Draw(texture, position, sourceRect, color, rotation, origin, scale, sfx, 0);
        }
    }
}
