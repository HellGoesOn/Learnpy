using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.Drawing;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Learnpy.Content.Systems
{
    public class DrawSystem : ISystem
    {

        public void Execute(World gameState)
        {
            IEnumerable<Entity> w = gameState.ActiveEntities.Where(x => x.HasComponent<TransformComponent>());
            foreach (Entity e in w) {
                bool hasDrawData = e.HasComponent<DrawDataComponent>();
                ref var transform = ref e.GetComponent<TransformComponent>();
                if (e.HasComponent<OpacityComponent>()) {
                    ref OpacityComponent comp = ref e.GetComponent<OpacityComponent>();

                    comp.CurrentValue = MathHelper.SmoothStep(comp.CurrentValue, comp.TargetValue, comp.Speed);
                }
                if(e.HasComponent<SpinComponent>()) {
                    ref var spin = ref e.GetComponent<SpinComponent>();
                    transform.Rotation += spin.Amount;
                }
                if (e.HasComponent<ChangeTintComponent>() && hasDrawData) {
                    ref var drawData = ref e.GetComponent<DrawDataComponent>();
                    ref var tintChange = ref e.GetComponent<ChangeTintComponent>();

                    drawData.Tint = Color.Lerp(drawData.Tint, tintChange.TargetTint, tintChange.Amount);
                }
            }

        }

        public void Render(LearnGame gameRenderer)
        {
            World w = gameRenderer.Worlds[gameRenderer.GameState];

            IEnumerable<Entity> mainList = w.ActiveEntities.Where(x => x.HasComponent<TransformComponent>()
            && x.HasComponent<TextureComponent>());

            foreach (Entity e in mainList.Where(x => x.HasComponent<PuzzleComponent>())) {
                TransformComponent transform = e.GetComponent<TransformComponent>();
                TextureComponent texturePath = e.GetComponent<TextureComponent>();

                PuzzleComponent puzzle = e.GetComponent<PuzzleComponent>();
                Texture2D texture = Assets.GetTexture(texturePath.Name);

                Rectangle rect = new Rectangle(0, 64 * (int)puzzle.PieceType, 128, 64);

                string finalText = Util.SpliceText(puzzle.StoredText, 8);
                Renderer.Draw(texture, transform.Position, rect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None);
                var origin = Assets.DefaultFontSmall.MeasureString(finalText);
                Renderer.DrawText(finalText, transform.Position + new Vector2(64, 32), Assets.DefaultFontSmall, Color.Purple, transform.Rotation, Vector2.One, origin * 0.5f, SpriteEffects.None);
            }

            foreach (Entity e in mainList.Where(x => !x.HasComponent<PuzzleComponent>())) {
                TransformComponent transform = e.GetComponent<TransformComponent>();
                TextureComponent texturePath = e.GetComponent<TextureComponent>();
                Texture2D texture = Assets.GetTexture(texturePath.Name);

                Vector2 scale = Vector2.One;
                Vector2 origin = Vector2.Zero;
                Color clr = Color.White;
                if (e.HasComponent<DrawDataComponent>()) {
                    DrawDataComponent drawData = e.GetComponent<DrawDataComponent>();
                    scale = drawData.Size;
                    origin = drawData.Origin;
                    clr = drawData.Tint;
                }

                float opacity = e.HasComponent<OpacityComponent>() ? e.GetComponent<OpacityComponent>().CurrentValue : 1.0f;
                float rot = transform.Rotation;

                //Rectangle rect = e.HasComponent<>?

                Renderer.Draw(texture, transform.Position, null, clr * opacity, rot, origin, scale, SpriteEffects.None);
            }
        }
    }
}
