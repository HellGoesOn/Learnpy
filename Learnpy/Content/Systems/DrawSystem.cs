using Learnpy.Content.Components;
using Learnpy.Content.Scenes;
using Learnpy.Core.Drawing;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
            List<Action> actionQueue = new List<Action>();
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
                if(e.Has<AnimationComponent>()) {
                    AnimationComponent c = e.Get<AnimationComponent>();
                    c.Action?.Invoke();

                    if (c.OnEndAction != null)
                        actionQueue.Add(c.OnEndAction);
                }

                if (e.Has<TextComponent>()) {
                    TextComponent c = e.Get<TextComponent>();
                    c.Action?.Invoke();
                }
            }

            foreach (Action action in actionQueue) {
                action.Invoke();
            }

            actionQueue.Clear();
        }

        public void Render(LearnGame gameRenderer)
        {
            World w = gameRenderer.Worlds[gameRenderer.GameState];

            IEnumerable<Entity> mainList = w.ActiveEntities.Where(x => x.Has<TransformComponent>()
            && x.Has<TextureComponent>());

            foreach (Entity e in mainList/*.Where(x => !x.Has<PuzzleComponent>())*/) {
                TransformComponent transform = e.Get<TransformComponent>();
                TextureComponent texturePath = e.Get<TextureComponent>();
                Texture2D texture = Assets.GetTexture(texturePath.Name);

                Vector2 scale = Vector2.One;
                Vector2 origin = Vector2.Zero;
                Color clr = Color.White;
                SpriteEffects sfx = SpriteEffects.None;
                if (e.Has<DrawDataComponent>()) {
                    DrawDataComponent drawData = e.Get<DrawDataComponent>();
                    scale = drawData.Scale;
                    origin = drawData.Origin;
                    clr = drawData.Tint;
                    sfx = drawData.SpriteEffects;
                }
                Rectangle? frame = null;

                if (e.Has<AnimationComponent>()) {
                    AnimationComponent animation = e.Get<AnimationComponent>();

                    if(animation.Frames != null && animation.Frames.Length > 0)
                        frame = animation.Frames[animation.CurrentFrame];
                }

                float opacity = e.Has<OpacityComponent>() ? e.Get<OpacityComponent>().CurrentValue : 1.0f;
                float rot = transform.Rotation;

                Renderer.Draw(texture, transform.Position, frame, clr * opacity, rot, origin, scale, sfx);

                if (e.Has<TextComponent>()) {
                    var texts = e.Get<TextComponent>();
                    
                    foreach(TextContext t in texts.Texts) {
                        Renderer.DrawText(t.Text, transform.Position + t.Position + t.ShadowOffset, t.Font, t.ShadowColor * opacity * t.Opacity, t.Rotation, t.Scale, t.Origin, SpriteEffects.None);
                        Renderer.DrawText(t.Text, transform.Position + t.Position, t.Font, t.Color * opacity * t.Opacity, t.Rotation, t.Scale, t.Origin, SpriteEffects.None);
                    }
                }
            }
        }
    }
}
