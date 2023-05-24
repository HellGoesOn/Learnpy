using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text.RegularExpressions;

namespace Learnpy
{
    public static class Util
    {
        public static void DrawRectangle(SpriteBatch sb, Vector2 position, Vector2 size, Color clr = default, float depth = 0f)
        {
            sb.Draw(Assets.GetTexture("Pixel"), position, new Rectangle(0, 0, (int)size.X, (int)size.Y), clr, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
        }

        public static string SpliceText(string text, int lineLength)
        {
            return Regex.Replace(text, "(.{" + lineLength + "})" + ' ', "$1" + Environment.NewLine);
        }

        public static string MatchBetween(string inputText, string precedingText = "", string start = "\"", string end = "\"")
        {
            string escapedStart = Regex.Escape(start); 
            string escapedEnd = Regex.Escape(end);
            string pattern = $"(?<={precedingText}{escapedStart})(.*?)(?={escapedEnd})";
            string match = Regex.Match(inputText, pattern, RegexOptions.Singleline).Value;
            return match;
        }
    }
}
