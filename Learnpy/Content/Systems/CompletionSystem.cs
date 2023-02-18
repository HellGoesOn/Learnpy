using Learnpy.Core;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Learnpy.Content.Systems
{
    public class CompletionSystem : ISystem
    {
        // ECS violation
        public string CurrentTarget = "";

        public string Task = "";

        public double Time;

        public int PrintTime;

        public bool Succesful;

        public int LevelTarget = 1;

        public void Execute(World gameState)
        {
            if (Input.ReleasedKey(Keys.Q))
            {
                RunCodeSystem rcs = gameState.GetSystem<RunCodeSystem>();
                if (rcs.CurrentSentence == CurrentTarget)
                {
                    PrintTime = 300;
                    Succesful = true;
                }
            }

            if(Input.PressedKey(Keys.Enter) && Succesful)
            {
                if(LevelTarget < SentenceFromText.levelFileNames.Length)
                {
                    SentenceFromText.Load(gameState, LevelTarget);
                    PrintTime = 0;
                    LevelTarget++;
                }
                else {
                    LevelTarget = 1;
                    EntryPoint.Instance.GameState = GameState.MainMenu;
                    EntryPoint.Instance.ResetWorld();
                }
            }

            if(!Succesful)
                Time += 0.016;

            if (PrintTime > 0)
                PrintTime--;
        }

        public void Render(LearnGame gameRenderer)
        {
            TimeSpan t = TimeSpan.FromSeconds(Time);
            Vector2 size = Assets.DefaultFont.MeasureString(Task) + new Vector2(10, 0);
            Util.DrawRectangle(gameRenderer.spriteBatch, new Vector2(495, 595), size, Color.Black * 0.75f);
            gameRenderer.spriteBatch.DrawString(Assets.DefaultFont, $"Время: {t.ToString(@"mm\:ss")}", new Vector2(900, 20), Color.LightGoldenrodYellow, 0f, Vector2.Zero, new Vector2(1), SpriteEffects.None, 1f);
            gameRenderer.spriteBatch.DrawString(Assets.DefaultFont, $"{Task}", new Vector2(500, 600), Color.LightGoldenrodYellow, 0f, Vector2.Zero, new Vector2(1), SpriteEffects.None, 1f);
            gameRenderer.spriteBatch.DrawString(Assets.DefaultFont, $"{Task}", new Vector2(501, 601), Color.Black, 0f, Vector2.Zero, new Vector2(1), SpriteEffects.None, 0.9f);
            if (PrintTime > 0)
            {
                Util.DrawRectangle(gameRenderer.spriteBatch, new Vector2(0, 260), new Vector2(1500, 200), Color.Black * 0.3f, 0.9f);
                gameRenderer.spriteBatch.DrawString(Assets.DefaultFontBig, "УСПЕХ!", new Vector2(520, 300), Color.Lime, 0f, Vector2.Zero, new Vector2(1), SpriteEffects.None, 1f);
                gameRenderer.spriteBatch.DrawString(Assets.DefaultFont, $"Затрачено времени: {t.ToString(@"mm\:ss")}", new Vector2(500, 400), Color.LightGoldenrodYellow, 0f, Vector2.Zero, new Vector2(1), SpriteEffects.None, 1f);
                gameRenderer.spriteBatch.DrawString(Assets.DefaultFont, $"Нажмите Enter, чтобы продолжить", new Vector2(500, 420), Color.LightGoldenrodYellow, 0f, Vector2.Zero, new Vector2(1), SpriteEffects.None, 1f);
            }
        }
        }
    }
