﻿using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Learnpy.Content.Systems
{
    public class DrawSystem : ISystem
    {

        public void Execute(World gameState)
        {
        }

        public void Render(LearnGame gameRenderer)
        {
            World w = gameRenderer.MainWorld;

            foreach (Entity e in w.ActiveEntities.Where(x => x.HasComponent<TransformComponent>()
            && x.HasComponent<TextureComponent>()
            && x.HasComponent<PuzzleComponent>()))
            {
                TransformComponent transform = e.GetComponent<TransformComponent>();
                TextureComponent texturePath = e.GetComponent<TextureComponent>();
                PuzzleComponent puzzle = e.GetComponent<PuzzleComponent>();
                Texture2D texture = Assets.GetTexture(texturePath.Name);

                Rectangle rect = new Rectangle(0, 64 * (int)puzzle.PieceType, 128, 64);

                string finalText = Util.SpliceText(puzzle.StoredText, 8);
                gameRenderer.spriteBatch.Draw(texture, transform.Position, rect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                var origin = Assets.DefaultFontSmall.MeasureString(finalText);
                gameRenderer.spriteBatch.DrawString(Assets.DefaultFontSmall, finalText, transform.Position + new Vector2(64, 32), Color.Purple, 0f, origin * 0.5f, Vector2.One, SpriteEffects.None, 1f);

                /*
                if(Collision.BoundingBox(e.GetComponent<BoxComponent>().Box, Input.MouseBox).Overlapped)
                {
                    string theWholeDeal = $"Type: {puzzle.PieceType}" +
                        $"\nCanConnect: {puzzle.CanConnect}" +
                        $"\nCanBeConnectedTo: {puzzle.CanBeConnectedTo}" +
                        $"\nConnectedTo: {puzzle.ConnectedTo}" +
                        $"\nConnectionTo: {puzzle.ConnectionTo}" +
                        $"\nMoveable: {e.HasComponent<MoveableComponent>()}";

                    gameRenderer.spriteBatch.DrawString(Assets.DefaultFont, theWholeDeal, Input.MouseBox.Centre + new Vector2(0, 16), Color.BlanchedAlmond, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
                    gameRenderer.spriteBatch.DrawString(Assets.DefaultFont, theWholeDeal, Input.MouseBox.Centre + new Vector2(1, 17), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                }*/
            }
        }
    }
}