using Learnpy.Content.Components;
using Learnpy.Content.Scenes.Transitions;
using Learnpy.Content.Systems;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Scenes
{
    public partial class LearnGame : Game
    {
        private float cyberTime;

        public void InitializeCyberspace()
        {
            World cyberSpace = Worlds[GameState.Cyberspace];

            var backdrop = cyberSpace.Create();
            backdrop.Add(new TextureComponent("Pixel"));
            backdrop.Add(new TransformComponent(new Vector2(0)));
            backdrop.Add(new DrawDataComponent(new Vector2(0, 0), new Vector2(1360f, 768f), 1, Color.Black));
            backdrop.Add(new OpacityComponent(1, 1f, 0.1f));

            cyberSpace.AddSystem<DrawSystem>();
            cyberSpace.AddSystem<DialogueSystem>();
            cyberSpace.AddSystem<MenuSystem>();
            cyberSpace.AddSystem<VelocitySystem>();

            var cam = Worlds[GameState.Cyberspace].camera;
            cam.zoom = 5f;
            cam.centre = new Vector2(1360, 768) * cam.zoom * 0.5f;

            var player = cyberSpace.Create();
            player.Add(new TransformComponent(new Vector2(0, 0)));
            player.Add(new TextureComponent("MC"));
            player.Add(new AnimationComponent(new[] {
                new Rectangle(0, 64, 32, 32),
                new Rectangle(0, 32, 32, 32),
                new Rectangle(0, 0, 32, 32)
            }));
            player.Add(new DrawDataComponent(new Vector2(16), Vector2.One));

            var bug = cyberSpace.Create();
            bug.Add(new TransformComponent(0, 0));
            bug.Add(new TextureComponent("Enemy"));
            bug.Add(new AnimationComponent() {
                Action = () =>
                {
                    bug.Get<TransformComponent>().Position += new Vector2(0, (float)Math.Sin(cyberTime));
                    cyberTime += 0.15f;
                }
            });
            bug.Add(new OpacityComponent(0, 0, 0));
            bug.Add(new DrawDataComponent(new Vector2(18, 21), Vector2.One) {
                SpriteEffects = SpriteEffects.FlipHorizontally
            });

            var options = cyberSpace.Create();
            options.Add(new OpacityComponent(0f, 0f, 0.1f));
            options.Add(new MenuComponent(new[] {
                new MenuOption("ucant", () =>
                {
            var backdrop = cyberSpace.Create();
            backdrop.Add(new TextureComponent("Pixel"));
            backdrop.Add(new TransformComponent(new Vector2(0)));
            backdrop.Add(new DrawDataComponent(new Vector2(0, 0), new Vector2(1360f, 768f), 1, Color.Crimson));
                    backdrop.Add(new OpacityComponent(0, 1f, 0.1f));
                    bug.Add(new VelocityComponent(new Vector2(-0.35f, 0)));

            options.Add(new OpacityComponent(0f, 0f, 0.1f));
                })
                {LocalePath ="monologue.txt"},
                new MenuOption("try", () =>
                {
                    bug.Add(new VelocityComponent(new Vector2(1.25f, -1.75f)));
                    bug.Add(new SpinComponent(0.26f));
                    sceneTransitions.Add(new FadeToBlack(GameState, GameState.Cyberspace) {FadeOutSpeed = 0.02f, FadeInSpeed = 0.1f,
                    SceneChange = () => { PartTwo(); } });
                })
                {LocalePath ="monologue.txt"}
            }) {
                IsSelected = false,
                Font = Assets.DefaultFontBig
            });
            options.Add(new TransformComponent(new Vector2(680, 480)));
            options.Add(new OpacityComponent(0f, 0f, 0));

            var diag = cyberSpace.Create();
            diag.Add(new DialogueComponent(new[] {
                "...",
                Locale.GetTranslation("mysteries", "monologue.txt"),
                Locale.GetTranslation("shadow", "monologue.txt"),
                Locale.GetTranslation("taxes", "monologue.txt"),
                Locale.GetTranslation("important", "monologue.txt"),
                Locale.GetTranslation("is", "monologue.txt"),
                Locale.GetTranslation("bruh", "monologue.txt")
            }) { Font = Assets.DefaultFontBig, AutoScroll = true, Speed = 4f, CenteredOrigin = true, Color = Color.Wheat, TimeUntilNextPageMax = 180,
            OnDialogueEnd = () =>
            {
                player.Add(new TransformComponent(640, 384));
                bug.Add(new TransformComponent(720, 374));
                player.Add(new OpacityComponent(0.0f, 1.0f, 0.1f));
                bug.Add(new OpacityComponent(0.0f, 1.0f, 0.1f));
                options.Add(new OpacityComponent(0f, 1f, 0.1f));
                diag.Add(new OpacityComponent(1f, 0.0f, 0.1f));
                options.Get<MenuComponent>().IsSelected = true;
            }
        });
            diag.Add(new TransformComponent(new Vector2(680, 580)));
            options.Add(new AnimationComponent() {
                Action = () =>
                {
                    player.Get<AnimationComponent>().CurrentFrame = options.Get<MenuComponent>().SelectedIndex;
                }
            });

            PartTwo();
        }

        private float cameraRaiseTime;
        private bool startDiag;

        public void PartTwo()
        {
            needsSuccess = true;
            isSuccess = false;
            cameraRaiseTime = 0f;
            startDiag = false;
            World cyberSpace = Worlds[GameState.Cyberspace];
            cyberSpace.WipeWorld();
            cyberSpace.AddSystem<DrawSystem>();
            cyberSpace.AddSystem<DialogueSystem>();
            cyberSpace.AddSystem<MenuSystem>();
            cyberSpace.AddSystem<VelocitySystem>();
            cyberSpace.AddSystem<CodeTypingSystem>();
            var cam = Worlds[GameState.Cyberspace].camera;
            cam.zoom = 4f;
            cam.centre = new Vector2(1360, 768) * cam.zoom * 0.5f;

            var backdrop = cyberSpace.Create();
            backdrop.Add(new TextureComponent("Pixel"));
            backdrop.Add(new TransformComponent(new Vector2(0)));
            backdrop.Add(new DrawDataComponent(new Vector2(0, 0), new Vector2(1360f, 768f), 1, Color.Aqua));
            backdrop.Add(new ChangeTintComponent(Color.Aqua, 0.12f));
            backdrop.Add(new OpacityComponent(1, 1f, 0.1f));
            var backdrop2 = cyberSpace.Create();
            backdrop2.Add(new TextureComponent("Pixel"));
            backdrop2.Add(new TransformComponent(new Vector2(0, 384)));
            backdrop2.Add(new DrawDataComponent(new Vector2(0, 0), new Vector2(1360, 300), 1, Color.Gray));
            backdrop2.Add(new OpacityComponent(1, 1f, 0.1f));

            var player = cyberSpace.Create();
            player.Add(new TransformComponent(new Vector2(500, 380)));
            player.Add(new TextureComponent("MC"));
            player.Add(new AnimationComponent(new[] {
                new Rectangle(0, 0, 32, 32),
                new Rectangle(0, 32, 32, 32),
                new Rectangle(0, 64, 32, 32)
            }));

            player.Add(new DrawDataComponent(new Vector2(16), Vector2.One));
            var sun = cyberSpace.Create();
            sun.Add(new TextureComponent("Pixel"));
            sun.Add(new TransformComponent(new Vector2(800, 204)));
            sun.Add(new DrawDataComponent(new Vector2(0, 0), new Vector2(60f, 60f), 1, Color.Yellow));
            sun.Add(new OpacityComponent(1, 1f, 0.1f));
            sun.Add(new AnimationComponent() {
                Action = () =>
                {
                    if (cameraRaiseTime < 20f) {
                        cameraRaiseTime += 0.1f;

                        player.Get<TransformComponent>().Position.X += 0.45f;
                        cyberSpace.camera.centre.Y -= 1.2f;
                    }
                },
                OnEndAction = () =>
                {
                    if (!startDiag && cameraRaiseTime >= 20f) {
                        startDiag = true;
                        var diag = cyberSpace.Create();
                        diag.Add(new DialogueComponent(new[] {
                "> Сегодня замечательный день.",
                "> Вы, как никогда, готовы получать всё новые и новые знания",
                "> С сегодняшенго дня, Вы будете изучать Python в совершенно новой Академии:" +
                "\n\"­«Пик Надежды.»\"",
                "> Что Вас ждёт внутри стен этой академии?",
                "> Сможете ли Вы наконец постичь Python?",
                "> Есть только один способ это выяснить..."
            }) {
                            Font = Assets.DefaultFont, AutoScroll = true, Speed = 4f, CenteredOrigin = true, Color = Color.Yellow, TimeUntilNextPageMax = 120,
                            OnDialogueEnd = () =>
                            {
                                cyberSpace.Destroy(diag.Id);
                                var bug = cyberSpace.Create();
                                Vector2 playerPos = player.Get<TransformComponent>().Position;
                                bug.Add(new TransformComponent(playerPos + new Vector2(140, -20)));
                                bug.Add(new TextureComponent("Enemy"));
                                bug.Add(new AnimationComponent() {
                                    Action = () =>
                                    {
                                        bug.Get<TransformComponent>().Position += new Vector2(0, (float)Math.Sin(cyberTime) * 0.15f);
                                        cyberTime += 0.15f;
                                    }
                                });
                                bug.Add(new OpacityComponent(-1.75f, 0.56f, 0.1f));
                                bug.Add(new DrawDataComponent(new Vector2(18, 21), Vector2.One, 1f, Color.White) {
                                    SpriteEffects = SpriteEffects.FlipHorizontally
                                });
                                backdrop.Add(new ChangeTintComponent(Color.Crimson, 0.05f));
                                sun.Add(new ChangeTintComponent(Color.DarkRed, 0.05f));
                                backdrop2.Add(new ChangeTintComponent(new Color(0.212f, 0.086f, 0.129f), 0.05f));
                                SeemsLikeItWillHaveToWait(cyberSpace, player, bug, backdrop, backdrop2, sun);
                            }
                        });
                        diag.Add(new TransformComponent(new Vector2(680, 280)));

                    }
                }
            });

        }

        private bool isSuccess;
        private bool needsSuccess = true;

        public void SeemsLikeItWillHaveToWait(World cyberSpace, Entity player, Entity bug, Entity backdrop, Entity backdrop2, Entity sun)
        {
            var textReader = cyberSpace.Create();
            textReader.Add(new TextInputComponent("", true));
            textReader.Add(new TextureComponent("Pixel"));
            textReader.Add(new TransformComponent());
            textReader.Add(new AnimationComponent() {
                Action = () =>
                 {
                     if (Input.ReleasedKey(Keys.Delete)) {
                         if (textReader.Get<TextInputComponent>().Text == "print('Hello World!')") {
                             isSuccess = true;
                         } else {
                             textReader.Get<TextInputComponent>().Text = "";
                         }
                     }
                 },
                OnEndAction = () =>
                {
                    if (isSuccess && needsSuccess) {
                        needsSuccess = isSuccess = false;
                        bug.Add(new SpinComponent(0.36f));
                        bug.Add(new OpacityComponent(0.56f, 0.0f, 0.05f));
                        VictoryDialogue(cyberSpace, player, backdrop, backdrop2, sun);
                    }
                }
            });
            var diag = cyberSpace.Create();
            diag.Add(new DialogueComponent(new[] {
                "     ",
                "> Похоже, перед тем как Вы сможете продолжить свой путь, Вам придётся кое-с-чем разобраться.",
                "> То, что Вы сейчас наблюдаете, нечто иное, как \"Ошибка\".",
                "> Они появляются повсюду, а виной тому - плохой код, написанный неграмотными людьми.",
                "> Конечно, Вас это не касается.",
                "> Вам придётся сразиться с этим \"Багом\".",
                "> Без паники, судя по всему, конкретно этот индивид - крайне слаб.",
                "> Для победы над ним, напишите свою первую программу - \"Hello World!\" с помощью функции print().",
                "> Когда вы будете уверены в набранном коде, просто нажмите Delete на клавиатуре.",
                "> Даже если вы ошибётесь, вы сможете попробовать ещё раз просто повторно нажав Delete."
            }) {
                Font = Assets.DefaultFont, AutoScroll = true, Speed = 4f, CenteredOrigin = true, Color = Color.Yellow, TimeUntilNextPageMax = 120,
                OnDialogueEnd = () =>
                {
                    cyberSpace.Destroy(diag.Id);
                    player.Get<AnimationComponent>().CurrentFrame = 1;
                    cyberSpace.GetSystem<CodeTypingSystem>().isEditingText = true;
                    Input.StartTextInput("");
                }
            });
            diag.Add(new TransformComponent(new Vector2(680, 280)));
        }

        public void VictoryDialogue(World cyberSpace, Entity player, Entity backdrop, Entity backdrop2, Entity sun)
        {
            var diag = cyberSpace.Create();
            diag.Add(new DialogueComponent(new[] {
                "     ",
                "> Этот Баг Вам не ровня!",
                "> Если продолжите схватывать всё на лету - сможете окочнить \"Пик Надежды\" досрочно!",
                "> А в этом случае, Вам гарантирован успех в жизне.",
                "> Учтите, что также как и в программировании, Вам придётся решать и другие виды задач," +
                "\nа баги могут быть опаснее.",
                "> Будьте готовы и удачи!"
            }) {
                Font = Assets.DefaultFont, AutoScroll = true, Speed = 3f, CenteredOrigin = true, Color = Color.Yellow, TimeUntilNextPageMax = 120,
                OnDialogueEnd = () =>
                {
                cyberSpace.Destroy(diag.Id);
                player.Get<AnimationComponent>().CurrentFrame = 0;
                player.Add(new VelocityComponent(new Vector2(1, 0)));
                cyberSpace.GetSystem<CodeTypingSystem>().isEditingText = true;
                Input.StartTextInput("");
                backdrop.Add(new ChangeTintComponent(Color.Aqua, 0.025f));
                backdrop2.Add(new ChangeTintComponent(Color.Gray, 0.025f));
                sun.Add(new ChangeTintComponent(Color.Yellow, 0.025f));
                EntryPoint.Instance.sceneTransitions.Add(new FadeToBlack(GameState.Cyberspace, GameState.Playground) {
                    FadeOutSpeed = 0.005f,
                    FadeInSpeed = 0.05f
                });
                }
            });
            diag.Add(new TransformComponent(new Vector2(680, 280)));
        }
    }
}
