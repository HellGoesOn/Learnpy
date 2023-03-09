using Learnpy.Content.Scenes;
using Learnpy.Core.Drawing;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Learnpy.Content.Scenes.Transitions
{
    public class FadeToBlack : ISceneTransition
    {
        public float FadeOutSpeed { get; set; }
        public float FadeInSpeed { get; set; }
        public float Fade { get; set; }
        public bool finished;

        public GameState TargetScene;
        public GameState CurrentScene;

        public Color Color { get; set; }
        public Action TransitionDone { get; set; }
        public Action SceneChange { get; set; }

        private bool flip;

        public FadeToBlack(GameState current, GameState target)
        {
            FadeInSpeed = FadeOutSpeed = 0.05f;
            Fade = -1.0f;
            CurrentScene = current;
            TargetScene = target;
            Color = Color.Black;
        }

        public void Draw(LearnGame game)
        {
            World cWorld = game.Worlds[CurrentScene];
            World nWorld = game.Worlds[TargetScene];
            if(!flip)
                Renderer.DrawScene(cWorld, cWorld.camera, Renderer.MainTarget);
            else
                Renderer.DrawScene(nWorld, nWorld.camera, Renderer.MainTarget);
            Renderer.Draw(() =>
            {
                Renderer.Draw(Assets.GetTexture("Pixel"), new Rectangle(0, 0, GameOptions.ScreenWidth, GameOptions.ScreenHeight), null, Color * Fade);
            }, null);
        }

        public bool IsFinished()
        {
            return finished;
        }

        public void Update(LearnGame game)
        {
            if (!flip) {
                if (Fade >= 2f) {
                    flip = true;
                }
                Fade += FadeOutSpeed;
            } else {
                SceneManager.SwitchScene(TargetScene);
                if (Fade >= 0.0f)
                    Fade -= FadeInSpeed;
                else {
                    finished = true;
                }
            }
        }

        public Action OnTransitionEnd()
        {
            return TransitionDone;
        }

        public bool SceneChanged()
        {
            return flip;
        }

        public Action OnSceneChanged()
        {
            return SceneChange;
        }
    }
}
