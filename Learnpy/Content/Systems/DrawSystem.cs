using Learnpy.Content.Components;
using Learnpy.Content.Scenes;
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
            IEnumerable<Entity> w = gameState.ActiveEntities.Where(x => x.Has<TransformComponent>());
            foreach (Entity e in w) {
                bool hasDrawData = e.Has<DrawDataComponent>();
                ref var transform = ref e.Get<TransformComponent>();
                if (e.Has<OpacityComponent>()) {
                    ref OpacityComponent comp = ref e.Get<OpacityComponent>();

                    comp.CurrentValue = MathHelper.SmoothStep(comp.CurrentValue, comp.TargetValue, comp.Speed);
                }
                if(e.Has<SpinComponent>()) {
                    ref var spin = ref e.Get<SpinComponent>();
                    transform.Rotation += spin.Amount;
                }
                if (e.Has<ChangeTintComponent>() && hasDrawData) {
                    ref var drawData = ref e.Get<DrawDataComponent>();
                    ref var tintChange = ref e.Get<ChangeTintComponent>();

                    drawData.Tint = Color.Lerp(drawData.Tint, tintChange.TargetTint, tintChange.Amount);
                }
            }

        }

        public void Render(LearnGame gameRenderer)
        {
            World w = gameRenderer.Worlds[gameRenderer.GameState];

            IEnumerable<Entity> mainList = w.ActiveEntities.Where(x => x.Has<TransformComponent>()
            && x.Has<TextureComponent>());

            foreach (Entity e in mainList.Where(x => x.Has<PuzzleComponent>())) {
                TransformComponent transform = e.Get<TransformComponent>();
                TextureComponent texturePath = e.Get<TextureComponent>();

                PuzzleComponent puzzle = e.Get<PuzzleComponent>();
                Texture2D texture = Assets.GetTexture(texturePath.Name);

                Rectangle rect = new Rectangle(0, 64 * (int)puzzle.PieceType, 128, 64);

                string finalText = Util.SpliceText(puzzle.StoredText, 8);
                Renderer.Draw(texture, transform.Position, rect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None);
                var origin = Assets.DefaultFontSmall.MeasureString(finalText);
                Renderer.DrawText(finalText, transform.Position + new Vector2(64, 32), Assets.DefaultFontSmall, Color.Purple, transform.Rotation, Vector2.One, origin * 0.5f, SpriteEffects.None);
            }

            foreach (Entity e in mainList.Where(x => !x.Has<PuzzleComponent>())) {
                TransformComponent transform = e.Get<TransformComponent>();
                TextureComponent texturePath = e.Get<TextureComponent>();
                Texture2D texture = Assets.GetTexture(texturePath.Name);

                Vector2 scale = Vector2.One;
                Vector2 origin = Vector2.Zero;
                Color clr = Color.White;
                if (e.Has<DrawDataComponent>()) {
                    DrawDataComponent drawData = e.Get<DrawDataComponent>();
                    scale = drawData.Scale;
                    origin = drawData.Origin;
                    clr = drawData.Tint;
                }
                Rectangle? frame = null;

                if (e.Has<AnimationComponent>()) {
                    AnimationComponent animation = e.Get<AnimationComponent>();
                    frame = animation.Frames[animation.CurrentFrame];
                }

                float opacity = e.Has<OpacityComponent>() ? e.Get<OpacityComponent>().CurrentValue : 1.0f;
                float rot = transform.Rotation;

                Renderer.Draw(texture, transform.Position, frame, clr * opacity, rot, origin, scale, SpriteEffects.None);
            }
        }
    }
}
