using Learnpy.Content.Components;
using Learnpy.Content.Scenes;
using Learnpy.Core.Drawing;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Learnpy.Content.Systems
{
    public class MenuSystem : ISystem
    {
        private float sway;
        private float time;

        public void Execute(World gameState)
        {
            sway = (float)Math.Sin(time) * 0.15f;
            time += 0.05f;

            List<Action> actionsToInvoke = new List<Action>();
            foreach (Entity e in gameState.ActiveEntities.Where(x => x.Has<MenuComponent>())) {
                ref var menu = ref e.Get<MenuComponent>();

                ref var opt = ref menu.Options[menu.SelectedIndex];
                if (!menu.IsSelected || SDL.SDL_IsTextInputActive() == SDL.SDL_bool.SDL_TRUE)
                    continue;

                if(Input.PressedKey(Keys.S) || Input.PressedKey(Keys.Down)) {
                    if(++menu.SelectedIndex >= menu.Options.Length) {
                        menu.SelectedIndex = 0;
                    }
                }

                if (Input.PressedKey(Keys.W) || Input.PressedKey(Keys.Up)) {
                    if (--menu.SelectedIndex < 0) {
                        menu.SelectedIndex = menu.Options.Length - 1;
                    }
                }

                if (Input.PressedKey(Keys.Enter) || Input.PressedKey(Keys.E)) {
                    actionsToInvoke.Add(menu.Options[menu.SelectedIndex].Action);
                }

                if (opt.ValueList != null) {
                    if(Input.PressedKey(Keys.D) || Input.PressedKey(Keys.Right)) {
                        if(++opt.SelectedValue >= opt.ValueList.Count) {
                            opt.SelectedValue = 0;
                        }
                    }

                    if(Input.PressedKey(Keys.A) || Input.PressedKey(Keys.Left)) {
                        if (--opt.SelectedValue < 0) {
                            opt.SelectedValue = opt.ValueList.Count-1;
                        }
                    }
                }
            }

            foreach (var action in actionsToInvoke) {
                action.Invoke();
            }
        }

        public void Render(LearnGame game)
        {
            World w = game.Worlds[game.GameState];
            foreach (Entity e in w.ActiveEntities.Where(x => x.Has<MenuComponent>())){
                int off = 0;
                ref MenuComponent comp = ref e.Get<MenuComponent>();

                ref var transform = ref e.Get<TransformComponent>();

                Vector2 positionOffset = new Vector2(GameOptions.ScreenWidth * 0.5f * 0.5f + 1, GameOptions.ScreenHeight * 0.5f * off + 1);
                if(e.Has<TransformComponent>()) {
                    positionOffset = transform.Position;
                }

                foreach (var option in comp.Options) {
                    bool selected = comp.SelectedIndex == option.Index;
                    string txt = Locale.GetTranslation(option.Name, option.LocalePath);
                    string lang = GameOptions.Language;
                    SpriteFont font = comp.Font;
                    if (option.ValueList != null) {
                        txt += $": {option.ValueList[option.SelectedValue]}";
                    }

                    float opacity = e.Has<OpacityComponent>() ? e.Get<OpacityComponent>().CurrentValue : 1.0f;

                    Vector2 size = font.MeasureString(txt);
                    float rotation = selected ? sway / (txt.Length * 0.25f) : 0;
                    Renderer.RequestScreenDraw(() =>
                    {
                        off++;
                        Renderer.DrawText(txt, positionOffset + Vector2.One + new Vector2(0, size.Y * off), font, (selected ? Color.Red : Color.Black) * opacity, rotation, Vector2.One, size * 0.5f, SpriteEffects.None);
                       Renderer.DrawText(txt, positionOffset + new Vector2(0, size.Y * off), font, (selected ? Color.Yellow : Color.Wheat) * opacity, rotation, Vector2.One, size * 0.5f, SpriteEffects.None);
                   }); 
                }
            }
        }
    }
}
