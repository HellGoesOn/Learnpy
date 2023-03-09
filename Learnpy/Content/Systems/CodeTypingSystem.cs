using Learnpy.Core;
using Learnpy.Content.Scenes;
using Learnpy.Core.Drawing;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using Learnpy.Content.Components;

namespace Learnpy.Content.Systems
{
    public class CodeTypingSystem : ISystem
    {
        public bool isEditingText;

        private int heldTime;

        public void Execute(World gameState)
        {
            if (Input.PressedKey(Keys.Home)) {
                isEditingText = !isEditingText;

                if (isEditingText)
                    Input.StartTextInput("");
                else {
                    Input.StopTextInput(out string result);
                    foreach (Entity e in gameState.ActiveEntities.Where(x => x.Has<TextInputComponent>())) {
                        ref var text = ref e.Get<TextInputComponent>();
                        if(text.Active)
                            text.Text = result;
                    }
                }
            }

            if (!isEditingText)
                return;

            if (Input.PressedKey(Keys.Left) || (Input.HeldKey(Keys.Left) && heldTime >= 20)) {
                Input.textCursor--;
            } else if (Input.PressedKey(Keys.Right) || (Input.HeldKey(Keys.Right) && heldTime >= 20)) {
                Input.textCursor++;
            }

            if (Input.HeldKey(Keys.Left) || Input.HeldKey(Keys.Right)) {
                if (heldTime < 20)
                    heldTime++;
            } else {
                heldTime = 0;
            }

            Input.textCursor = (int)MathHelper.Clamp(Input.textCursor, 0, Input.editedString.Length);

        }

        public void Render(LearnGame gameRenderer)
        {
            if (!isEditingText)
                return;

            string lineUpToCursor = Input.editedString.Substring(0, Input.textCursor);
            string[] allLines = Input.editedString.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            int lineCountToCursor = lineUpToCursor.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Count();

            string fullline = string.Join(Environment.NewLine, allLines, 0, (int)MathHelper.Max(0, lineCountToCursor-1));
            int indexInLine = (int)MathHelper.Clamp(Input.textCursor - fullline.Length, 0, allLines[Math.Max(0, lineCountToCursor - 1)].Length);

            Texture2D pix = Assets.GetTexture("Pixel");
            Vector2 constOff = Assets.DefaultFont.MeasureString("1");

            float offX = constOff.X * indexInLine;


            Renderer.Draw(pix, new Vector2(60 + offX, 60 + constOff.Y * Math.Max(1, lineCountToCursor)), new Rectangle(0, 0, 8, 2), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None); 
            Renderer.DrawText($"CursorPos:{Input.textCursor}; HeldTime: {heldTime}; LineLength{allLines[Math.Max(0, lineCountToCursor - 1)].Length}", new Vector2(60, 40), Assets.DefaultFont, Color.White, 0f, Vector2.One, Vector2.Zero, SpriteEffects.None);
            Renderer.DrawText(Input.editedString, new Vector2(60, 60), Assets.DefaultFont, Color.White, 0f, Vector2.One, Vector2.Zero, SpriteEffects.None);
        }
    }
}
