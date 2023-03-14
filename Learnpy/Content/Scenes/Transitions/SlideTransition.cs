using Learnpy.Content.Scenes;
using Learnpy.Core.Drawing;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Learnpy.Content.Scenes.Transitions
{
    public class SlideTransition : ISceneTransition
    {
        public float SlideSpeed { get; set; } = 0.025f;
        public float Delay { get; set; } = 0.5f;
        public bool finished;

        public GameState TargetScene;
        public GameState CurrentScene;

        public Color Color { get; set; }
        public Action TransitionDone { get; set; }
        public Action SceneChange { get; set; }
        public IContext Context { get; set; }

        private float slideProgress;
        private float delay;
        private Vector2 position;
        private Vector2 basePosition;
        private Vector2 screenCoverPosition;
        private Vector2 endPosition;

        private bool flip;

        public SlideTransition(GameState current, GameState target, Direction direction = Direction.Right)
        {
            CurrentScene = current;
            TargetScene = target;
            Color = Color.Black;
            switch (direction) {
                case Direction.Right:
                    basePosition = new Vector2(-GameOptions.ScreenWidth, 0);
                    screenCoverPosition = new Vector2(0, 0);
                    endPosition = new Vector2(GameOptions.ScreenWidth, 0);
                    break;
                case Direction.Left:
                    basePosition = new Vector2(GameOptions.ScreenWidth, 0);
                    screenCoverPosition = new Vector2(0, 0);
                    endPosition = new Vector2(-GameOptions.ScreenWidth, 0);
                    break;
                case Direction.Up:
                    basePosition = new Vector2(0, GameOptions.ScreenHeight);
                    screenCoverPosition = new Vector2(0, 0);
                    endPosition = new Vector2(0, -GameOptions.ScreenHeight);
                    break;
                case Direction.Down:
                    basePosition = new Vector2(0, -GameOptions.ScreenHeight);
                    screenCoverPosition = new Vector2(0, 0);
                    endPosition = new Vector2(0, GameOptions.ScreenHeight);
                    break;
            }
    }

        public void Draw(LearnGame game)
        {
            position = Vector2.Lerp(flip ? screenCoverPosition : basePosition, flip ? endPosition : screenCoverPosition, Math.Max(0, slideProgress));
            World cWorld = game.Worlds[CurrentScene];
            World nWorld = game.Worlds[TargetScene];
            if(!flip)
                Renderer.DrawScene(cWorld, cWorld.camera, Renderer.MainTarget);
            else
                Renderer.DrawScene(nWorld, nWorld.camera, Renderer.MainTarget);
            Renderer.Draw(() =>
            {
                Renderer.Draw(Assets.GetTexture("Pixel"), position, new Rectangle(0, 0, GameOptions.ScreenWidth, GameOptions.ScreenHeight), Color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None);
            }, null);
        }

        public bool IsFinished()
        {
            return finished;
        }

        public void Update(LearnGame game)
        {
            if (!flip) {
                if (slideProgress >= 1f) {
                    flip = true;
                    slideProgress = 0;
                } else {
                    slideProgress += SlideSpeed;
                }
            } else {
                SceneManager.SwitchScene(TargetScene, Context);
                if (slideProgress <= 1f) {
                    if (delay < Delay) {
                        delay += SlideSpeed;
                    } else
                        slideProgress += SlideSpeed;
                } else {
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
