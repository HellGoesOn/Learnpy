using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Learnpy.Content
{
    public struct TextContext
    {
        public string Text { get; set; }
        public float Opacity { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; }
        public Color ShadowColor { get; set; }
        public Vector2 Position;
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 ShadowOffset { get; set; }
        public SpriteFont Font { get; set; }

        public TextContext(string text, Vector2 position, SpriteFont? font = null, Color? color = null, Color? shadowColor = null)
        {
            Text = text;
            Opacity = 1f;
            Rotation = 0f;
            Origin = Vector2.Zero;
            Scale = Vector2.One;
            ShadowOffset = Vector2.One;
            Position = position;
            Font = font == null ? Assets.DefaultFont : font;
            Color = color == null ? Color.White : color.Value;
            ShadowColor = color == null ? Color.Black : color.Value;
        }
    }
}
