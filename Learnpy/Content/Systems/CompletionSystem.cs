using Learnpy.Core.Drawing;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Learnpy.Content.Scenes;
using Learnpy.Content.Scenes.Transitions;

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
            if (Input.ReleasedKey(Keys.Q)) {
                RunCodeSystem rcs = gameState.GetSystem<RunCodeSystem>();
                if (rcs.CurrentSentence == CurrentTarget) {
                    PrintTime = 300;
                    Succesful = true;
                }
            }

            if (Input.PressedKey(Keys.Enter) && Succesful) {
                if (LevelTarget < SentenceFromText.levelFileNames.Length) {
                    SentenceFromText.Load(gameState, LevelTarget);
                    PrintTime = 0;
                    LevelTarget++;
                } else {
                    LevelTarget = 1;
                    EntryPoint.Instance.sceneTransitions.Add(new FadeToBlack(GameState.Playground, GameState.MainMenu));
                }
            }

            if (!Succesful)
                Time += 0.016;

            if (PrintTime > 0)
                PrintTime--;
        }

        public void Render(LearnGame gameRenderer)
        {
            TimeSpan t = TimeSpan.FromSeconds(Time);
            Vector2 size = Assets.DefaultFont.MeasureString(Task) + new Vector2(10, 0);
            Util.DrawRectangle(gameRenderer.spriteBatch, new Vector2(395, 495), size, Color.Black * 0.75f);
            Renderer.DrawText($"{Locale.Translations["time"]}: {t.ToString(@"mm\:ss")}", new Vector2(900, 20), Assets.DefaultFont, Color.LightGoldenrodYellow, 0f, new Vector2(1), Vector2.Zero, SpriteEffects.None);
            Renderer.DrawText($"{Task}", new Vector2(401, 501), Assets.DefaultFont, Color.Black, 0f, new Vector2(1), Vector2.Zero, SpriteEffects.None);
            Renderer.DrawText($"{Task}", new Vector2(400, 500), Assets.DefaultFont, Color.LightGoldenrodYellow, 0f,  new Vector2(1), Vector2.Zero, SpriteEffects.None);
            if (PrintTime > 0) {
                Util.DrawRectangle(gameRenderer.spriteBatch, new Vector2(0, 260), new Vector2(1500, 200), Color.Black * 0.3f, 0.9f);
                Renderer.DrawText($"{Locale.Translations["success"]}", new Vector2(520, 300), Assets.DefaultFont, Color.Lime, 0f,  new Vector2(1), Vector2.Zero, SpriteEffects.None);
                Renderer.DrawText($"Нажмите Enter, чтобы продолжить", new Vector2(500, 420), Assets.DefaultFont, Color.LightGoldenrodYellow, 0f, new Vector2(1), Vector2.Zero, SpriteEffects.None);
                Renderer.DrawText($"Затрачено времени: {t.ToString(@"mm\:ss")}", new Vector2(500, 400), Assets.DefaultFont, Color.LightGoldenrodYellow, 0f,  new Vector2(1), Vector2.Zero, SpriteEffects.None);
            }
        }
    }
}
