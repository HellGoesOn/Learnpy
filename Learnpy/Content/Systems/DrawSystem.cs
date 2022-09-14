using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            foreach (int e in w.EntitiesById)
            {
                TransformComponent transform = w.Entities[e].GetComponent<TransformComponent>();
                TextureComponent texturePath = w.Entities[e].GetComponent<TextureComponent>();
                PuzzleComponent puzzle = w.Entities[e].GetComponent<PuzzleComponent>();
                Texture2D texture = Assets.GetTexture(texturePath.Name);

                Rectangle rect = new Rectangle(0, 64 * (int)puzzle.PieceType, 128, 64);

                gameRenderer.spriteBatch.Draw(texture, transform.Position, rect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                var origin = Assets.DefaultFont.MeasureString(puzzle.StoredText);
                gameRenderer.spriteBatch.DrawString(Assets.DefaultFont, puzzle.StoredText, transform.Position + new Vector2(64, 32), Color.Purple, 0f, origin * 0.5f, Vector2.One, SpriteEffects.None, 1f);
            }
        }
    }
}
