using Learnpy.Content.Components;
using Learnpy.Content.Scenes;
using Learnpy.Core.Drawing;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Systems
{
    public class DialogueSystem : ISystem
    {
        public void Execute(World gameState)
        {
            List<Action> actions = new List<Action>();
            foreach (Entity e in gameState.ActiveEntities.Where(x => x.Has<DialogueComponent>())) {
                ref var diag = ref e.Get<DialogueComponent>();

                if (diag.Progress <= 1f)
                    diag.Progress += diag.Speed;
                else {
                    if (diag.DisplayedLetters < diag.Pages[diag.CurrentPage].Length) {
                        diag.DisplayedLetters = (int)MathHelper.Clamp(diag.DisplayedLetters + diag.Progress, 0, diag.Pages[diag.CurrentPage].Length);
                        diag.Progress = 0.0f;
                    }
                }

                if (diag.Selected && (Input.PressedKey(Keys.E) || Input.PressedKey(Keys.Enter))) {
                    if (diag.CurrentPage < diag.Pages.Length-1) {
                        diag.CurrentPage++;
                        diag.DisplayedLetters = 0;
                        diag.Progress = 0.0f;
                    } else {
                        diag.Selected = false;
                        diag.OnDialogueEnd.Invoke();
                    }
                }

                if (diag.AutoScroll && diag.DisplayedLetters >= diag.Pages[diag.CurrentPage].Length) {
                    if(++diag.TimeUntilNextPage >= diag.TimeUntilNextPageMax) {
                        if (diag.CurrentPage < diag.Pages.Length - 1) {
                            diag.CurrentPage++;
                            diag.DisplayedLetters = 0;
                            diag.Progress = 0.0f;
                            diag.TimeUntilNextPage = 0;
                        } else {
                            diag.AutoScroll = false;
                            actions.Add(diag.OnDialogueEnd);
                        }
                    }
                }
            }

            foreach (Action a in actions)
                a?.Invoke();
        }

        public void Render(LearnGame gameRenderer)
        {
            foreach (Entity e in gameRenderer.Worlds[gameRenderer.GameState].ActiveEntities.
                Where(x => x.Has<TransformComponent>() && x.Has<DialogueComponent>())) {

                var pos = e.Get<TransformComponent>();
                var text = e.Get<DialogueComponent>();

                SpriteFont font = text.Font;
                float opacity = e.Has<OpacityComponent>() ? e.Get<OpacityComponent>().CurrentValue : 1f;

                string txt = text.Pages[text.CurrentPage].Substring(0, text.DisplayedLetters);
                float rotation = pos.Rotation;
                Vector2 origin = text.CenteredOrigin ? font.MeasureString(txt) * 0.5f : Vector2.Zero;

                Renderer.RequestScreenDraw(() =>
                {
                    Renderer.DrawText(txt, pos.Position + Vector2.One, font, Color.Black * opacity, rotation, Vector2.One, origin, SpriteEffects.None);
                    Renderer.DrawText(txt, pos.Position, font, text.Color * opacity, rotation, Vector2.One, origin, SpriteEffects.None);
                });
            }
        }
    }
}
