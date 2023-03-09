using Learnpy.Content.Scenes;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Learnpy.Core.Drawing
{
    public static class Renderer
    {
        static SpriteBatch batch => EntryPoint.Instance.spriteBatch;

        public static RenderTarget2D MainTarget;

        public static List<Action> drawRequests = new List<Action>();

        private static LearnGame game => EntryPoint.Instance;

        public static void DrawText(string text, Vector2 position, SpriteFont font, Color color, float rotation, Vector2 scale, Vector2 origin, SpriteEffects spriteEffects)
        {
            batch.DrawString(font, text, position, color, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects sfx)
        {
            batch.Draw(texture, position, sourceRect, color, rotation, origin, scale, sfx, 0);
        }

        public static void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRect, Color color)
        {
            batch.Draw(texture, destinationRectangle, sourceRect, color);
        }

        public static void DrawScene(World world, Camera camera, RenderTarget2D target)
        {
            game.GraphicsDevice.SetRenderTarget(target);
            game.GraphicsDevice.Clear(Color.DarkSeaGreen);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);
            world.Draw(EntryPoint.Instance);
            batch.End();
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            foreach(Action a in drawRequests) {
                a.Invoke();
            }
            batch.End();
            drawRequests.Clear();
        }

        /// <summary>
        /// Used to request to draw something unaffected by camera
        /// </summary>
        /// <param name="drawAction"></param>
        public static void RequestScreenDraw(Action drawAction)
        {
            drawRequests.Add(drawAction);
        }

        public static void Draw(Action drawAction, Camera camera)
        {
            if(camera != null)
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);
            else
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            drawAction.Invoke();
            batch.End();
        }
    }
}
