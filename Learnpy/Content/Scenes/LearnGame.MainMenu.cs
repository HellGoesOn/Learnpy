using Learnpy.Content.Components;
using Learnpy.Content.Systems;
using Microsoft.Xna.Framework;
using Learnpy.Content;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Learnpy.Core.ECS;
using Learnpy.Core.Extensions;
using Learnpy.Content.Scenes.Transitions;
using Learnpy.Core;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Learnpy.Content.Scenes
{
    public partial class LearnGame
    {
        private List<Entity> bullets = new List<Entity>();

        private float time;

        public void InitializeMainMenu()
        {
            bullets.Clear();
            SoundEngine.StartMusic("MainTheme", true);

            Input.StopTextInput(out var useless);

            MainMenu.AddSystem<DrawSystem>();
            MainMenu.AddSystem<MenuSystem>();
            MainMenu.AddSystem<VelocitySystem>();
            MainMenu.AddSystem<DialogueSystem>();

            var backdrop = MainMenu.Create();
            backdrop.Add(new TextureComponent("Pixel"));
            backdrop.Add(new TransformComponent(new Vector2(0)));
            backdrop.Add(new DrawDataComponent(new Vector2(0, 0), new Vector2(1360, 768), 1, new Color(0.463f, 0.11f, 0.11f)));


            var backdrop2 = MainMenu.Create();
            backdrop2.Add(new TextureComponent("Pixel"));
            backdrop2.Add(new TransformComponent(new Vector2(0, 368)));
            backdrop2.Add(new DrawDataComponent(new Vector2(0, 0), new Vector2(1360, 400), 1, new Color(0.212f, 0.086f, 0.129f)));
            backdrop2.Add(new OpacityComponent(-1.75f, 1.1f, 0.1f));

            var logo = MainMenu.Create();
            logo.Add(new TransformComponent(680, 294));
            logo.Add(new TextureComponent("1662"));
            logo.Add(new DrawDataComponent(new Vector2(79, 28), new Vector2(3), 1f, Color.White) { SpriteEffects = SpriteEffects.FlipHorizontally});
            logo.Add(new OpacityComponent(-1.75f, 1.1f, 0.1f));
            logo.Add(new AnimationComponent() {
                Action = () =>
                {
                    ref var pos = ref logo.Get<TransformComponent>();
                    pos.Position += new Vector2(0, (float)Math.Sin(time) * 0.5f);
                    time += 0.35f;
                }
            });

            float spinSpeed = 0.002f;
            var spinningDrum = MainMenu.Create();
            spinningDrum.Add(new TextureComponent("Drum"));
            spinningDrum.Add(new SpinComponent(spinSpeed));
            spinningDrum.Add(new TransformComponent(new Vector2(1280, 700)));
            spinningDrum.Add(new DrawDataComponent(new Vector2(36.5f, 31.5f), new Vector2(6f), 1, Color.Black));
            spinningDrum.Add(new OpacityComponent(-1.75f, 1.1f, 0.1f));

            float[] rotations = new float[]
                {
                    -MathHelper.PiOver2 + MathHelper.PiOver4 + 0.18f,
                    -MathHelper.PiOver2,
                    -MathHelper.PiOver2 -MathHelper.PiOver4 - 0.18f,
                    MathHelper.PiOver2 + MathHelper.PiOver4 + 0.18f,
                    MathHelper.PiOver2,
                    MathHelper.PiOver2 - MathHelper.PiOver4 - 0.18f
                };
            float[] yOffsets = new float[] {
                78,
                78,
                78,
                78,
                78,
                78
            };
            for (int i = 0; i < 6; i++) {
                var bullet = MainMenu.Create();
                bullet.Add(new TransformComponent(new Vector2(1280, 700), rotations[i]));
                bullet.Add(new TextureComponent("Bullet"));
                bullet.Add(new DrawDataComponent(new Vector2(6, yOffsets[i]), new Vector2(5), 1f, Color.Black));
                bullet.Add(new OpacityComponent(-1.75f, 1.1f, 0.1f));
                bullet.Add(new SpinComponent(spinSpeed));
                bullets.Add(bullet);
            }

            var mainMenu = MainMenu.Create();
            mainMenu.Add(new OpacityComponent(-1.75f, 1.1f, 0.1f));
            var startOption = new MenuOption("start", () =>
            {
            });
            startOption.Action = () =>
            {
                if (GameOptions.Language == "error") {
                    mainMenu.Get<MenuComponent>().Options[0].Name = "err";
                    return;
                }

                mainMenu.Get<MenuComponent>().Options[0].Name = "start";
                /*GameState = GameState.Playground;*/
                sceneTransitions.Add(new SlideTransition(GameState, GameState.Cyberspace, (Direction)new Random().Next((int)Direction.Down+1)) {
                    Color = Color.Black,
                    SlideSpeed = 0.1f,
                    Delay = 1f,
                    Context = new CombatContext() {
                        BulletCount = 5,
                        EnemyCount = 3
                    }
                });

                mainMenu.Add(new OpacityComponent(1f, 0.0f, 0.15f));
                mainMenu.Get<MenuComponent>().IsSelected = false;
            };
            mainMenu.Add(new TransformComponent(new Vector2(680, 404)));
            mainMenu.Add(new MenuComponent
                (startOption,
                new MenuOption("options", () =>
                {
                    mainMenu.Get<MenuComponent>().IsSelected = false;
                    mainMenu.Get<TransformComponent>().Position -= new Vector2(500, 0);
                    mainMenu.Get<MenuComponent>().Options[0].Name = "start";
                    var options = MainMenu.Create();
                    options.Add(new TransformComponent(new Vector2(680, 384)));

                    var volume = new MenuOption("volume", () =>
                    {
                        var cmp = options.Get<MenuComponent>();
                        var val = GameOptions.Volumes[cmp.Options[cmp.SelectedIndex].SelectedValue];
                        GameOptions.Volume = val;
                    }, true);
                    foreach (var v in GameOptions.Volumes)
                        volume.ValueList.Add(v);

                    volume.SelectedValue = GameOptions.Volumes.FindIndex(x=>x == GameOptions.Volume);

                    var language = new MenuOption("language", () =>
                    {
                        var cmp = options.Get<MenuComponent>();
                        var val = cmp.Options[cmp.SelectedIndex].Value;
                        GameOptions.Language = val.ToString();
                        Locale.Fill();
                        SentenceFromText.Init();
                        SentenceFromText.Load(MainWorld, 0);
                    }, true);
                    language.ValueList.Add("ru");
                    language.ValueList.Add("en");
                    language.ValueList.Add("error");

                    language.SelectedValue = language.ValueList.FindIndex(x => x.ToString() == GameOptions.Language);


                    var resolution = new MenuOption("resolution", () =>
                    {
                    }, true);
                    GameOptions.Resolutions.ForEach(x => resolution.ValueList.Add(x));
                    resolution.Action = () =>
                    {
                        var cmp = options.Get<MenuComponent>();
                        var val = GameOptions.Resolutions[cmp.Options[cmp.SelectedIndex].SelectedValue];
                        var wd = Regex.Match(val.ToString(), "^.*(?=x)").Value;
                        int width = int.Parse(wd);
                        var h = Regex.Match(val.ToString(), "(?<=x).*").Value;
                        int height = int.Parse(h);
                        GameOptions.ScreenWidth = width;
                        GameOptions.ScreenHeight = height;
                        GameOptions.NeedsUpdate = true;
                    };
                    resolution.SelectedValue = GameOptions.Resolutions.FindIndex(
                        x => x.ToString() == $"{GameOptions.ScreenWidth}x{GameOptions.ScreenHeight}");

                    options.Add(new MenuComponent(resolution, language, volume,
                        new MenuOption("back", () =>
                        {
                            MainMenu.Destroy(options.Id);
                            mainMenu.Get<MenuComponent>().IsSelected = true;
                            mainMenu.Get<TransformComponent>().Position += new Vector2(500, 0);
                        })) { IsSelected = true, Font = Assets.DefaultFontBig }
                        );
                }),
                new MenuOption("exit", () =>
                {
                    foreach(Entity e in bullets) {
                        var rot = e.Get<TransformComponent>().Rotation;
                        e.Remove<SpinComponent>();
                        e.Add(new VelocityComponent(new Vector2(0, -16).RotateBy(rot)));
                    }
                    ref var f = ref mainMenu.Get<MenuComponent>();
                    f.IsSelected = false;

                    this.Exit();
                })) {
                IsSelected = true,
                Font = Assets.DefaultFontBig
            });

        }
    }
}
