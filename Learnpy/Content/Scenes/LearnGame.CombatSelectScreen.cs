using Learnpy.Content.Components;
using Learnpy.Content.Scenes.Transitions;
using Learnpy.Content.Systems;
using Learnpy.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Scenes
{
    public partial class LearnGame : Game
    {
        int combatSelectScreen_SelectedIndex = 1;
        float combatSelectScreen_swipeValue;
        int combatSelectScreen_swipeDirection;
        string[] combatSelectScreen_levelPaths;
        public void InitCombatSelectScreen()
        {
            SoundEngine.StartMusic("MainTheme", true);
            combatSelectScreen_SelectedIndex = 1;
            var w = Worlds[GameState.CombatSelect];
            var yScale = Assets.DefaultFont.MeasureString("D").Y;
            var moveSpeed = 4;

            var backdrop = w.Create();
            backdrop.Add(new TextureComponent("Pixel"));
            backdrop.Add(new TransformComponent(new Vector2(0)));
            backdrop.Add(new DrawDataComponent(new Vector2(0, 0), new Vector2(1360f, 768f), 1, Color.Black));
            backdrop.Add(new OpacityComponent(1, 1f, 0.1f));
            backdrop.Add(new AnimationComponent() {
                Action = ()=>
                {
                    ref var txtComponent = ref backdrop.Get<TextComponent>();
                    if (combatSelectScreen_swipeValue > 0) {
                        for (int i = 1; i < txtComponent.Texts.Length; i++) {
                            txtComponent.Texts[i].Position.Y += moveSpeed * combatSelectScreen_swipeDirection;
                        }
                        combatSelectScreen_swipeValue -= moveSpeed;
                    } else {
                        if(Input.PressedKey(Keys.Escape)) {
                            sceneTransitions.Add(new SlideTransition(GameState, GameState.MainMenu, (Direction)new Random().Next((int)Direction.Down + 1)) {
                                Color = Color.Black,
                                SlideSpeed = 0.02f
                            });
                        }

                        if (Input.PressedKey(Keys.S) || Input.PressedKey(Keys.Down)) {
                            if (++combatSelectScreen_SelectedIndex >= txtComponent.Texts.Length) {
                                combatSelectScreen_swipeDirection = 1;
                                combatSelectScreen_swipeValue = yScale * (txtComponent.Texts.Length - 2);
                                combatSelectScreen_SelectedIndex = 1;
                            } else {
                                combatSelectScreen_swipeDirection = -1;
                                combatSelectScreen_swipeValue = yScale;
                            }
                        }

                        if (Input.PressedKey(Keys.W) || Input.PressedKey(Keys.Up)) {
                            if (--combatSelectScreen_SelectedIndex < 1) {
                                combatSelectScreen_swipeDirection = -1;
                                combatSelectScreen_swipeValue = yScale * (txtComponent.Texts.Length-2);
                                combatSelectScreen_SelectedIndex = txtComponent.Texts.Length - 1;
                            } else {
                                combatSelectScreen_swipeDirection = 1;
                                combatSelectScreen_swipeValue = yScale;
                            }
                        }

                        if(Input.PressedKey(Keys.E) || Input.PressedKey(Keys.Enter)) {
                            SoundEngine.StartMusic("SMT", true);
                            sceneTransitions.Add(new SlideTransition(GameState, GameState.Combat, (Direction)new Random().Next((int)Direction.Down + 1)) {
                                Color = Color.Black,
                                SlideSpeed = 0.02f,
                                Context = new CombatContext() {
                                    BulletCount = 5,
                                    EnemyCount = 2,
                                    LessonPath = combatSelectScreen_levelPaths[combatSelectScreen_SelectedIndex-1]
                                },
                            });
                        }
                    }
                }
            });
            backdrop.Add(new TextComponent(new TextContext("Выберите урок:", new Vector2(40, 384), Assets.DefaultFontBig, Color.Gold, Color.Goldenrod)));

            var line = w.Create();
            line.Add(new TextureComponent("Pixel"));
            line.Add(new DrawDataComponent(new Vector2(0.5f, 0), new Vector2(250, 1), 1, Color.White));
            line.Add(new TransformComponent(680, 384));
            line.Add(new OpacityComponent(1, 1f, 0.1f));

            var bottomLine = w.Create();
            bottomLine.Add(new TextureComponent("Pixel"));
            bottomLine.Add(new DrawDataComponent(new Vector2(0.5f, 0), new Vector2(250, 1), 1, Color.White));
            bottomLine.Add(new TransformComponent(680, 384 + yScale));
            bottomLine.Add(new OpacityComponent(1, 1f, 0.1f));

            w.AddSystem<DrawSystem>();
            w.AddSystem<DialogueSystem>();
            w.AddSystem<MenuSystem>();
            w.AddSystem<VelocitySystem>();

            const string basePath = "Content/ru/Combat/Lessons/";
            string[] allLevelPaths = Directory.GetFiles(basePath);
            combatSelectScreen_levelPaths = new string[allLevelPaths.Length];
            int index = 1;
            int lvl = 0;
            foreach(var levelPath in allLevelPaths) {
                string text = File.ReadAllText(levelPath);
                combatSelectScreen_levelPaths[lvl] = Path.GetFileNameWithoutExtension(levelPath);
                string name = Util.MatchBetween(text, "Name=");
                ref var txtComponent = ref backdrop.Get<TextComponent>();
                var measurement = Assets.DefaultFont.MeasureString(name);
                Array.Resize(ref txtComponent.Texts, txtComponent.Texts.Length + 1);
                txtComponent.Texts[index] = new TextContext(name, new Vector2(680, 384 - measurement.Y + (index * measurement.Y))) {
                    Font = Assets.DefaultFont,
                    Origin = new Vector2(measurement.X * 0.5f, 0),
                    Color = Color.Goldenrod
                };
                index++;
                lvl++;
            }

            
        }
    }
}
