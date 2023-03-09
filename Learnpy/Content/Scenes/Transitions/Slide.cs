using Learnpy.Core.Drawing;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Scenes.Transitions
{
    public class Slide : ISceneTransition
    {
        public float Speed { get; set; }
        public float Fade { get; set; }
        public bool finished;

        public GameState TargetScene;
        public GameState CurrentScene;

        private bool flip;

        public Slide(GameState current, GameState target)
        {
            Speed = 0.05f;
            Fade = -1.0f;
            CurrentScene = current;
            TargetScene = target;
        }

        public void Draw(LearnGame game)
        {
            World cWorld = game.Worlds[CurrentScene];
            World nWorld = game.Worlds[TargetScene];
            if (!flip)
                Renderer.DrawScene(cWorld, cWorld.camera, Renderer.MainTarget);
            else
                Renderer.DrawScene(nWorld, nWorld.camera, Renderer.MainTarget);
            Renderer.Draw(() =>
            {
                Renderer.Draw(Assets.GetTexture("Pixel"), cWorld.camera.Position, null, Color.White * Fade, 0f, Vector2.Zero, new Vector2(GameOptions.ScreenWidth, GameOptions.ScreenHeight), SpriteEffects.None);
            }, game.Worlds[CurrentScene].camera);
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
                Fade += Speed;
            } else {
                game.GameState = TargetScene;
                if (Fade >= 0.0f)
                    Fade -= Speed;
                else {
                    finished = true;
                }
            }
        }
    }
}
