using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Learnpy.Content.Systems
{
    public class MenuSystem : ISystem
    {
        public void Execute(World gameState)
        {
            List<Action> actionsToInvoke = new List<Action>();
            foreach (int e in gameState.EntitiesById) {
                var entity = gameState.Entities[e];
                ref var menu = ref entity.GetComponent<MenuComponent>();

                ref var opt = ref menu.Options[menu.SelectedIndex];
                if (!menu.IsSelected)
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
            foreach (Entity e in w.ActiveEntities.Where(x => x.HasComponent<MenuComponent>())){
                int off = 0;
                ref MenuComponent comp = ref e.GetComponent<MenuComponent>();

                ref var transform = ref e.GetComponent<TransformComponent>();

                Vector2 positionOffset = Vector2.Zero;
                if(e.HasComponent<TransformComponent>()) {
                    positionOffset = transform.Position;
                }

                foreach (var option in comp.Options) {
                    bool selected = comp.SelectedIndex == off;
                    string txt = Locale.Translations[option.Name];
                    string lang = GameOptions.Language;

                    if (option.ValueList != null) {
                        txt += $": {option.ValueList[option.SelectedValue]}";
                    }

                    Vector2 size = Assets.DefaultFont.MeasureString(txt);
                    game.spriteBatch.DrawString(Assets.DefaultFont,txt, new Vector2(GameOptions.ScreenWidth * 0.5f - size.X * 0.5f, GameOptions.ScreenHeight * 0.5f + size.Y * off) + positionOffset, selected ? Color.Yellow : Color.Wheat, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1.0f);
                    game.spriteBatch.DrawString(Assets.DefaultFont,txt, new Vector2(GameOptions.ScreenWidth * 0.5f - size.X * 0.5f + 1, GameOptions.ScreenHeight * 0.5f + size.Y * off+ 1) + positionOffset, selected ? Color.Red : Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
                    off++;
                }
            }
        }
    }
}
