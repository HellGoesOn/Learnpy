using Learnpy.Content.Components;
using Learnpy.Content.Systems;
using Microsoft.Xna.Framework;
using Learnpy.Content;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Learnpy.Core.ECS;
using Learnpy.Core.Extensions;

namespace Learnpy.Core
{
    public partial class LearnGame
    {
        private List<Entity> bullets = new List<Entity>();

        private bool dies;
        private int deathTime;
        private int startTime;
        private bool start;

        public void InitializeMainMenu()
        {
            MainMenu.AddSystem<DrawSystem>();
            MainMenu.AddSystem<MenuSystem>();
            MainMenu.AddSystem<VelocitySystem>();
            MainMenu.AddCollection<MenuComponent>();
            MainMenu.AddCollection<TransformComponent>();
            MainMenu.AddSystem<CodeTypingSystem>();

            var backdrop = MainMenu.Create();

            backdrop.AddComponent(new TextureComponent("Pixel"));
            backdrop.AddComponent(new TransformComponent(new Vector2(0)));
            backdrop.AddComponent(new DrawDataComponent(new Vector2(0, 0), new Vector2(1360f, 768f), 1, Color.DarkRed));

            float spinSpeed = 0.002f;
            var spinningDrum = MainMenu.Create();
            spinningDrum.AddComponent(new TextureComponent("Drum"));
            spinningDrum.AddComponent(new SpinComponent(spinSpeed));
            spinningDrum.AddComponent(new TransformComponent(new Vector2(1280, 700)));
            spinningDrum.AddComponent(new DrawDataComponent(new Vector2(36.5f, 31.5f), new Vector2(6f), 1, Color.Black));
            spinningDrum.AddComponent(new OpacityComponent(-1.75f, 1.1f, 0.1f));

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
                bullet.AddComponent(new TransformComponent(new Vector2(1280, 700), rotations[i]));
                bullet.AddComponent(new TextureComponent("Bullet"));
                bullet.AddComponent(new DrawDataComponent(new Vector2(6, yOffsets[i]), new Vector2(5), 1f, Color.Black));
                bullet.AddComponent(new OpacityComponent(-1.75f, 1.1f, 0.1f));
                bullet.AddComponent(new SpinComponent(spinSpeed));
                bullets.Add(bullet);
            }

            var mainMenu = MainMenu.Create();
            mainMenu.AddComponent(new OpacityComponent(-1.75f, 1.1f, 0.1f));
            var startOption = new MenuOption("start", () =>
            {
            });
            startOption.Action = () =>
            {
                if (GameOptions.Language == "error") {
                    mainMenu.GetComponent<MenuComponent>().Options[0].Name = "err";
                    return;
                }

                mainMenu.GetComponent<MenuComponent>().Options[0].Name = "start";
                GameState = GameState.Playground;
            };
            mainMenu.AddComponent(new TransformComponent());
            mainMenu.AddComponent(new MenuComponent
                (startOption,
                new MenuOption("options", () =>
                {
                    mainMenu.GetComponent<MenuComponent>().IsSelected = false;
                    mainMenu.GetComponent<TransformComponent>().Position = new Vector2(-300, 0);
                    mainMenu.GetComponent<MenuComponent>().Options[0].Name = "start";
                    var options = MainMenu.Create();

                    var language = new MenuOption("language", () =>
                    {
                        var cmp = options.GetComponent<MenuComponent>();
                        var val = cmp.Options[cmp.SelectedIndex].Value;
                        GameOptions.Language = val.ToString();
                        Locale.Fill();
                        SentenceFromText.Init();
                        SentenceFromText.Load(MainWorld, 0);
                    }, true);
                    language.ValueList.Add("ru");
                    language.ValueList.Add("en");
                    language.ValueList.Add("error");


                    var resolution = new MenuOption("resolution", () =>
                    {
                    }, true);
                    GameOptions.Resolutions.ForEach(x => resolution.ValueList.Add(x));
                    resolution.Action = () =>
                    {
                        var cmp = options.GetComponent<MenuComponent>();
                        var val = cmp.Options[cmp.SelectedIndex].Value;
                        var wd = Regex.Match(val.ToString(), "^.*(?=x)").Value;
                        int width = int.Parse(wd);
                        var h = Regex.Match(val.ToString(), "(?<=x).*").Value;
                        int height = int.Parse(h);
                        GameOptions.ScreenWidth = width;
                        GameOptions.ScreenHeight = height;
                        GameOptions.NeedsUpdate = true;
                    };
                    options.AddComponent(new MenuComponent(resolution, language,
                        new MenuOption("back", () =>
                        {
                            MainMenu.Destroy(options.Id);
                            mainMenu.GetComponent<MenuComponent>().IsSelected = true;
                            mainMenu.GetComponent<TransformComponent>().Position = new Vector2(0, 0);
                        })) { IsSelected = true }
                        );
                }),
                new MenuOption("exit", () =>
                {
                    foreach(Entity e in bullets) {
                        var rot = e.GetComponent<TransformComponent>().Rotation;
                        e.RemoveComponent<SpinComponent>();
                        e.AddComponent(new VelocityComponent(new Vector2(0, -16).RotateBy(rot)));
                    }
                    ref var f = ref mainMenu.GetComponent<MenuComponent>();
                    f.IsSelected = false;
                    dies = true;
                    deathTime = 180;
                })) {
                IsSelected = true
            });
        }
    }
}
