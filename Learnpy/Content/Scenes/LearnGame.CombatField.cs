using Learnpy.Content.Components;
using Learnpy.Content.Scenes.Transitions;
using Learnpy.Content.Systems;
using Learnpy.Core;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Scenes
{
    public partial class LearnGame : Game
    {
        int crosshairTarget;
        float dist = 360;
        bool showCode;
        bool ended;

        float showErrorTime;

        Entity playerField;
        Entity enemyField;
        Entity textBackDrop;

        List<Entity> enemies = new List<Entity>();
        List<Entity> combatBullets = new List<Entity>();
        List<Entity> bulletsOnStandBy = new List<Entity>();

        Vector2[] enemyPositionOffsets = new Vector2[] {
            new Vector2(240, -40),
            Vector2.Zero,
            new Vector2(240, 160),
        };

        public void BeginCombat(CombatContext context)
        {
            ended = false;
            showErrorTime = 0;
            showCode = false;

            if (context == null)
                context = new CombatContext();

            crosshairTarget = 0;
            enemies.Clear();
            combatBullets.Clear();
            bulletsOnStandBy.Clear();
            cyberTime = 0;
            dist = 300;
            Vector2 distFromCentre = new Vector2(dist, 0);

            World w = Worlds[GameState.Combat];
            w.AddSystem<DrawSystem>();
            w.AddSystem<VelocitySystem>();
            w.AddSystem<TextInputSystem>();


            var background = w.Create();
            background.Add(new TextureComponent("Pixel"));
            background.Add(new DrawDataComponent(Vector2.Zero, new Vector2(1360, 768), 1, Color.DarkRed));
            background.Add(new TransformComponent());
            background.Add(new AnimationComponent() {
                Action = () =>
                {
                    if (showErrorTime > 0)
                        showErrorTime -= 0.05f;

                    cyberTime += 0.15f;
                    if (dist < 740) {
                        dist += 5.0f;
                    }
                    distFromCentre = new Vector2(dist, 0);
                    ref var pos1 = ref playerField.Get<TransformComponent>();
                    pos1.Position = new Vector2(680, 384) - distFromCentre;
                    ref var pos2 = ref enemyField.Get<TransformComponent>();
                    pos2.Position = new Vector2(680, 384) + distFromCentre;

                    if(combatBullets.Count > 0 && !combatBullets[0].Has<VelocityComponent>())
                        ShootFunc(combatBullets[0]);
                },
                OnEndAction = () =>
                {
                    if (context.GetSuccessCondition().Invoke() && !ended) {
                        sceneTransitions.Add(new SlideTransition(GameState, context.ProceedsTo, (Direction)new Random().Next((int)Direction.Down + 1)) {
                            Color = Color.Black,
                            SlideSpeed = 0.02f
                        });

                        if (context.LessonPath == "ShootOut1")
                            Database.GiveAchievement("Привет Мир!");

                        ended = true;
                    }

                    if (context.GetFailCondition().Invoke() && !ended) {
                        sceneTransitions.Add(new SlideTransition(GameState, context.ReturnsTo, (Direction)new Random().Next((int)Direction.Down + 1)) {
                            Color = Color.Black,
                            SlideSpeed = 0.02f
                        });
                        ended = true;
                    }

                    if (Input.PressedKey(Keys.Delete)) {
                        showCode = !showCode;
                        ref var c = ref background.Get<TextInputComponent>();
                        c.Active = showCode;
                    }

                    if (combatBullets.Count <= 0) {
                        foreach (var bullet in bulletsOnStandBy) {
                            ref var pos = ref bullet.Get<TransformComponent>();
                            pos.Position = new Vector2(36.5f * 4 + 174, 134 + 60 * combatBullets.Count - 1);
                            bullet.Remove<VelocityComponent>();
                            combatBullets.Add(bullet);
                        }
                        bulletsOnStandBy.Clear();

                    } else {

                        if (combatBullets[0].Get<TransformComponent>().Position.X > 1360 + 64 * 10) {
                            if (enemies.Count > 0) {
                                string attemptedSolution = textBackDrop.Get<TextComponent>().Get(0).Text;
                                context.BulletCount--;
                                bool gotanswer = enemies[crosshairTarget].Get<RequirementComponent>().IsMatching(attemptedSolution);
                                if (gotanswer) {
                                    ref TextInputComponent t = ref textBackDrop.Get<TextInputComponent>();
                                    t.Text = "";
                                    w.Destroy(enemies[crosshairTarget].Id);
                                    enemies.RemoveAt(crosshairTarget);
                                    context.EnemyCount--;
                                    crosshairTarget = 0;
                                    if(combatBullets.Count <= 1) {
                                        context.BulletCount = 1;
                                        var bullet = bulletsOnStandBy[0];
                                        ref var pos = ref bullet.Get<TransformComponent>();
                                        pos.Position = new Vector2(36.5f * 4 + 174, 134 + 60 * combatBullets.Count - 1);
                                        bullet.Remove<VelocityComponent>();

                                        combatBullets.Add(bullet);
                                        bulletsOnStandBy.RemoveAt(0);
                                    }
                                }
                                else {
                                    errorText = enemies[crosshairTarget].Get<RequirementComponent>().GetText(attemptedSolution);
                                    showErrorTime = 10f;
                                }
                            }

                            combatBullets[0].Remove<VelocityComponent>();
                            combatBullets[0].Get<AnimationComponent>().Action = () => { };

                            bulletsOnStandBy.Add(combatBullets[0]);

                            SoundEngine.PlaySound("DrumSpin");

                            foreach (var bul in combatBullets) {
                                bul.Get<TransformComponent>().Position -= new Vector2(0, 60);
                            }

                            combatBullets.RemoveAt(0);
                        }
                    }
                }
            });

            playerField = w.Create();
            playerField.Add(new TextureComponent("Pixel"));
            playerField.Add(new DrawDataComponent(new Vector2(0.5f, 0.5f), new Vector2(2000, 800), 1f, new Color(0.212f, 0.086f, 0.129f)));
            playerField.Add(new TransformComponent(new Vector2(680, 384) -distFromCentre, (float)Math.PI * 0.33f));

            float spinSpeed = 0.002f;

            var controlSchemeText = "[E] - выстрел\n[Delete.] - ввод\nW/S - выбор цели";
            var playerDrum = w.Create();
            playerDrum.Add(new TextureComponent("Drum"));
            playerDrum.Add(new SpinComponent(spinSpeed));
            playerDrum.Add(new TransformComponent(new Vector2(36.5f*2, 31.5f*6)));
            playerDrum.Add(new DrawDataComponent(new Vector2(36.5f, 31.5f), new Vector2(4f), 1, Color.Black));
            playerDrum.Add(new TextComponent(new TextContext(controlSchemeText, new Vector2(46, 0)) {
                Color = Color.Lime,
                ShadowColor = Color.DarkGreen,
                Origin = Assets.DefaultFont.MeasureString(controlSchemeText) * 0.5f
            }));

            for (int i = 0; i < context.BulletCount; i++) {
                var bullet = w.Create();
                bullet.Add(new TextureComponent("Bullet"));
                bullet.Add(new TransformComponent(36.5f * 4 + 174, 134 + 60 * i, MathHelper.PiOver2));
                bullet.Add(new DrawDataComponent(new Vector2(6, 20), Vector2.One * 4f, 1f, Color.Black));
                bullet.Add(new ChangeTintComponent(Color.White, 1f));

                bullet.Add(new AnimationComponent());

                combatBullets.Add(bullet);
            }

            enemyField = w.Create();
            enemyField.Add(new TextureComponent("Pixel"));
            enemyField.Add(new DrawDataComponent(new Vector2(0.5f, 0.5f), new Vector2(2000, 800), 1f, new Color(0.212f, 0.086f, 0.129f)));
            enemyField.Add(new TransformComponent(new Vector2(680, 384) + distFromCentre, (float)Math.PI * 0.33f));

            var enemyDrum = w.Create();
            enemyDrum.Add(new TextureComponent("Drum"));
            enemyDrum.Add(new SpinComponent(-spinSpeed));
            enemyDrum.Add(new TransformComponent(new Vector2(1360, 768) - new Vector2(36.5f, 31.5f)));
            enemyDrum.Add(new DrawDataComponent(new Vector2(36.5f, 31.5f), new Vector2(4f), 1, Color.Black));

            var playerModel = w.Create();
            playerModel.Add(new TransformComponent(new Vector2(128 * 3, 460)));
            playerModel.Add(new TextureComponent("MC"));
            playerModel.Add(new AnimationComponent(new[] {
                new Rectangle(0, 32, 32, 32),
                new Rectangle(0, 0, 32, 32),
                new Rectangle(0, 64, 32, 32)
            }));
            playerModel.Add(new DrawDataComponent(new Vector2(16), Vector2.One * 6f));

            RequirementComponent[] requirements = new[]
            {
                new RequirementComponent("Вывести на экран\n 'Hello World!'", "print('Hello World!')"),
                new RequirementComponent("2 + 2 = ?", "4"),
                new RequirementComponent("Худший в мире?", "Олег")
            };

            MissReason[] missReasons = new[]
            {
                new MissReason() {
                    Text ="Неверный тип данных!\nОтвет должен быть числом!",
                    Condition = (string s) =>
                    {
                        bool res = int.TryParse(s, out var test);

                        return !res;
                    }
                },
                new MissReason() {
                    Text ="Введено слишком низкое значение!",
                    Condition = (string s) =>
                    {
                        int.TryParse(s, out var res);
                        return res <= -1000;
                    }
                }
            };


            CombatContextReader.ReadCombatContext(w, "Content/ru/Combat/Lessons/" + context.LessonPath, enemies);
            /*
            for (int i = 0; i < context.EnemyCount; i++) {
                var bug = w.Create();
                bug.Add(new TransformComponent(new Vector2(1000, 240) + enemyPositionOffsets[i]));
                bug.Add(new TextureComponent("Enemy"));
                bug.Add(new AnimationComponent() {
                    Action = () =>
                    {
                        bug.Get<TransformComponent>().Position += new Vector2(0, (float)Math.Sin(cyberTime));
                    }
                });
                bug.Add(new DrawDataComponent(new Vector2(18, 21), Vector2.One * 6f) {
                    SpriteEffects = SpriteEffects.FlipHorizontally
                });
                bug.Add(requirements[i]);
                bug.Add(new TextComponent(new TextContext(requirements[i].Description, Vector2.Zero) {
                    Origin = Assets.DefaultFont.MeasureString(requirements[i].Description) * 0.5f
                }));

                if (i == 1)
                    bug.Get<RequirementComponent>().MissReasons = missReasons;
                    enemies.Add(bug);
            }*/

            string combatText = "БИТВА В ПРОЦЕССЕ";
            var blackBar = w.Create();
            blackBar.Add(new TextureComponent("Pixel"));
            blackBar.Add(new DrawDataComponent(Vector2.Zero, new Vector2(1360, 60), 1, Color.Black));
            blackBar.Add(new TransformComponent());
            blackBar.Add(new TextComponent(new[] {
                new TextContext(combatText, new Vector2(1360, 20))
                {
                    Color = Color.DarkRed,
                    ShadowColor = Color.Crimson
                }
            }) {
                Action = () =>
                {
                    ref var texts = ref blackBar.Get<TextComponent>();
                    ref TextContext t1 = ref texts.Get(0);
                    t1.Opacity = 1f + (float)Math.Sin(t1.Position.X) * 0.15f;
                    if ((t1.Position.X -= 6f) < -Assets.DefaultFont.MeasureString(t1.Text).X) {
                        t1.Position.X = 1360;
                    }
                }
            });

            var blackBar2 = w.Create();
            blackBar2.Add(new TextureComponent("Pixel"));
            blackBar2.Add(new DrawDataComponent(Vector2.Zero, new Vector2(1360, 60), 1, Color.Black));
            blackBar2.Add(new TransformComponent(0, 708));
            blackBar2.Add(new TextComponent(new[] {
                new TextContext(combatText, new Vector2(-Assets.DefaultFont.MeasureString(combatText).X, 20))
                {
                    Color = Color.DarkRed,
                    ShadowColor = Color.Crimson
                }
            }) {
                Action = () =>
                {
                    ref var texts = ref blackBar2.Get<TextComponent>();
                    ref TextContext t1 = ref texts.Get(0);
                    t1.Opacity = 1f + (float)Math.Sin(t1.Position.X) * 0.15f;
                    if ((t1.Position.X += 6f) > 1360) {
                        t1.Position.X = -Assets.DefaultFont.MeasureString(t1.Text).X;
                    }
                }
            });

            var crosshair = w.Create();
            crosshair.Add(new TextureComponent("Crosshair"));
            crosshair.Add(new DrawDataComponent(new Vector2(20), Vector2.One * 32f, 0f, Color.Crimson));
            crosshair.Add(new TransformComponent(1000, 240));
            crosshair.Add(new SpinComponent(0.012f));
            crosshair.Add(new OpacityComponent(0, 1.0f, 0.00f));
            crosshair.Add(new AnimationComponent() {
              Action = () => {
                  ref var scale = ref crosshair.Get<DrawDataComponent>();
                  ref var opacity = ref crosshair.Get<OpacityComponent>();
                  ref var position = ref crosshair.Get<TransformComponent>();
                  if (!showCode) {
                      if (Input.PressedKey(Keys.Up) || Input.PressedKey(Keys.W)) {
                          if (++crosshairTarget > enemies.Count - 1)
                              crosshairTarget = 0;
                      }

                      if (Input.PressedKey(Keys.Down) || Input.PressedKey(Keys.S)) {
                          if (--crosshairTarget < 0)
                              crosshairTarget = enemies.Count - 1;
                      }
                  }

                  if(enemies.Count > 0)
                    position.Position = enemies[crosshairTarget].Get<TransformComponent>().Position;

                  if (w.ActiveEntities.Any(x => x.Has<VelocityComponent>())/* || enemies.Count <= 0*/) {
                      if (opacity.CurrentValue > 0f)
                          opacity.CurrentValue -= 0.1f;

                      if (scale.Scale.X < 32f || scale.Scale.Y < 32f)
                          scale.Scale *= 1.12f;

                  } else {
                      if (opacity.CurrentValue < 1f)
                          opacity.CurrentValue += 0.1f;

                      if (scale.Scale.X > 6f || scale.Scale.Y > 6f)
                          scale.Scale *= 0.88f;
                  }
            }
            });

            CreateBullletPapyrus(w);

            var errorMessage = w.Create();
            errorMessage.Add(new TextureComponent("Pixel"));
            errorMessage.Add(new DrawDataComponent(new Vector2(0.5f, 0.5f), new Vector2(1, 1), 1f, Color.Black));
            errorMessage.Add(new TransformComponent(680, 384));
            errorMessage.Add(new TextComponent(
                new TextContext("", Vector2.Zero)));
            errorMessage.Add(new AnimationComponent() {
                Action = () =>
                {
                    var measurement = Assets.DefaultFont.MeasureString(errorText);
                    ref var txt = ref errorMessage.Get<TextComponent>().Texts[0];
                    txt.Text = errorText;
                    txt.Color = Color.DarkRed;
                    txt.Origin = measurement * 0.5f;
                    txt.Font = Assets.DefaultFont;
                    txt.Scale = new Vector2(1.25f);
                    txt.Opacity = showErrorTime;
                    ref var dr = ref errorMessage.Get<DrawDataComponent>();
                    ref var op = ref errorMessage.Get<OpacityComponent>();
                    dr.Scale = measurement * 1.35f;
                    op.TargetValue = showErrorTime;
                }
            });
            errorMessage.Add(new OpacityComponent(0, 0, 1));
        }
        string errorText = "";
        private Entity ShootFunc(Entity bullet)
        {
            if (Input.PressedKey(Keys.E) && !showCode) {
                SoundEngine.PlaySound("Summon", GameOptions.Volume * 0.5f);
                bullet.Add(new VelocityComponent(64, 0));
                bullet.Add(new AnimationComponent()); // clean up any shenanigans
            }

            return bullet;
        }

        private void CreateBullletPapyrus(World w)
        {
            textBackDrop = w.Create();
            Vector2 defaultPos = new Vector2(680, 134);
            float size = 500;
            textBackDrop.Add(new TransformComponent(defaultPos));
            textBackDrop.Add(new TextureComponent("Pixel"));
            textBackDrop.Add(new DrawDataComponent(new Vector2(0.5f, 0.0f), new Vector2(800, 0), 1f, Color.Black));
            textBackDrop.Add(new OpacityComponent(0, 0, 0.15f));
            textBackDrop.Add(new TextInputComponent());
            textBackDrop.Add(new TextComponent(new[] {
                new TextContext("", new Vector2(-390, 10)) {
                Color = Color.Lime,
                },
                new TextContext("Нажмите Delete. чтобы подтвердить", new Vector2(-380, size - 30)) {
                    Font = Assets.DefaultFont,
                    Color = Color.DarkGreen
                }
            }));
            textBackDrop.Add(new AnimationComponent() {
                Action = () =>
                {
                    ref TextInputComponent tic = ref textBackDrop.Get<TextInputComponent>();
                    tic.Active = showCode;

                    ref TextComponent t = ref textBackDrop.Get<TextComponent>();
                    t.Get(0).Text = string.IsNullOrWhiteSpace(tic.Text) ? "" : tic.Text;

                    if (Input.PressedKey(Keys.Delete)) {
                        bool val = w.GetSystem<TextInputSystem>().isEditingText;
                        w.GetSystem<TextInputSystem>().isEditingText = !val;

                        if (!val) {
                            Input.StartTextInput(t.Get(0).Text);
                        } else {
                            Input.StopTextInput(out string result);
                        }
                    }

                    ref var dd = ref textBackDrop.Get<DrawDataComponent>();
                    ref var o = ref textBackDrop.Get<OpacityComponent>();
                    if (showCode) {
                        o.TargetValue = 1f;
                        if (dd.Scale.Y < size)
                            dd.Scale.Y += 12;
                    } else {
                        o.TargetValue = 0.0f;
                        if (dd.Scale.Y > 0)
                            dd.Scale.Y -= 12;
                    }
                }
            });

            var bulletTop = w.Create();
            bulletTop.Add(new TextureComponent("Bullet"));
            bulletTop.Add(new TransformComponent(defaultPos, MathHelper.PiOver2));
            bulletTop.Add(new DrawDataComponent(new Vector2(6, 20), Vector2.One * 4f, 1f));
            bulletTop.Add(new OpacityComponent(0, 0, 0.15f));
            bulletTop.Add(new AnimationComponent(new[] {new Rectangle(0, 0, 6, 40)}) {
                Action = () =>
                {
                    ref var o = ref bulletTop.Get<OpacityComponent>();
                    if (showCode) {
                        o.TargetValue = 1f;
                    } else {
                        o.TargetValue = 0.0f;
                    }
                }
            });

            var bulletBottom = w.Create();
            bulletBottom.Add(new TextureComponent("Bullet"));
            bulletBottom.Add(new TransformComponent(defaultPos + new Vector2(0, 24), MathHelper.PiOver2));
            bulletBottom.Add(new DrawDataComponent(new Vector2(6, 20), Vector2.One * 4f, 1f));
            bulletBottom.Add(new OpacityComponent(0, 0, 0.15f));
            bulletBottom.Add(new AnimationComponent(new[] { new Rectangle(6, 0, 6, 40) }) {
                Action = () =>
                {
                    ref var pos = ref bulletBottom.Get<TransformComponent>(); 
                    ref var o = ref bulletBottom.Get<OpacityComponent>();

                    if (showCode) {
                        o.TargetValue = 1f;
                        if (pos.Position.Y < defaultPos.Y + 28 + size)
                            pos.Position.Y += 12;
                    } else {
                        o.TargetValue = 0.0f;
                        if (pos.Position.Y > defaultPos.Y + 28)
                            pos.Position.Y -= 12;
                    }
                }
            });
        }
    }
}
