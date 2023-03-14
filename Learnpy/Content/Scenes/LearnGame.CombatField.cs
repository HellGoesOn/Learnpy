using Learnpy.Content.Components;
using Learnpy.Content.Scenes.Transitions;
using Learnpy.Content.Systems;
using Learnpy.Core;
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
        float dist = 360;
        Entity playerField;
        Entity enemyField;
        List<Entity> enemies = new List<Entity>();
        List<Entity> combatBullets = new List<Entity>();
        List<Entity> bulletsOnStandBy = new List<Entity>();
        Vector2[] enemyPositionOffsets = new Vector2[] {
            new Vector2(240, -40),
            Vector2.Zero,
            new Vector2(240, 160),
        };
        int crosshairTarget;
        public void BeginCombat(CombatContext context)
        {
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
            var background = w.Create();
            background.Add(new TextureComponent("Pixel"));
            background.Add(new DrawDataComponent(Vector2.Zero, new Vector2(1360, 768), 1, Color.DarkRed));
            background.Add(new TransformComponent());
            background.Add(new AnimationComponent() {
                Action = () =>
                {
                    cyberTime += 0.15f;
                    if (dist < 740) {
                        dist += 5.0f;
                    }
                    distFromCentre = new Vector2(dist, 0);
                    ref var pos1 = ref playerField.Get<TransformComponent>();
                    pos1.Position = new Vector2(680, 384) - distFromCentre;
                    ref var pos2 = ref enemyField.Get<TransformComponent>();
                    pos2.Position = new Vector2(680, 384) + distFromCentre;
                },
                OnEndAction = () =>
                {
                    if (combatBullets.Count <= 0) {
                        foreach (var bullet in bulletsOnStandBy) {
                            ref var pos = ref bullet.Get<TransformComponent>();
                            pos.Position = new Vector2(36.5f * 4 + 140, 134 + 60 * combatBullets.Count - 1);
                            bullet.Remove<VelocityComponent>();

                            combatBullets.Add(bullet);

                            if (combatBullets.Count == 1) {
                                ref var act = ref bullet.Get<AnimationComponent>();
                                act.Action = () =>
                                {
                                    if (Input.PressedKey(Keys.Space)) {
                                        SoundEngine.PlaySound("GunShot");
                                        bullet.Add(new VelocityComponent(64, 0));
                                    }
                                };
                            } else {
                                ref var act = ref bullet.Get<AnimationComponent>();
                                act.Action = () => { };
                            }
                        }

                        bulletsOnStandBy.Clear();
                    } else {

                        if (combatBullets[0].Get<TransformComponent>().Position.X > 1360 + 64 * 60) {
                            if (enemies.Count > 0) {
                                w.Destroy(enemies[crosshairTarget].Id);
                                enemies.RemoveAt(crosshairTarget);
                            }

                            combatBullets[0].Remove<VelocityComponent>();
                            combatBullets[0].Get<AnimationComponent>().Action = () => { };

                            crosshairTarget = 0;

                            bulletsOnStandBy.Add(combatBullets[0]);

                            SoundEngine.PlaySound("DrumSpin");

                            foreach (var bul in combatBullets) {
                                bul.Get<TransformComponent>().Position -= new Vector2(0, 60);
                            }

                            combatBullets.RemoveAt(0);
                            if (combatBullets.Count <= 0)
                                return;

                            var nextBullet = combatBullets[0];
                            ref var act = ref nextBullet.Get<AnimationComponent>();
                            act.Action = () =>
                            {
                                if (Input.PressedKey(Keys.Space)) {
                                    SoundEngine.PlaySound("GunShot");
                                    nextBullet.Add(new VelocityComponent(64, 0));
                                }
                            };
                        }
                    }
                }
            });

            playerField = w.Create();
            playerField.Add(new TextureComponent("Pixel"));
            playerField.Add(new DrawDataComponent(new Vector2(0.5f, 0.5f), new Vector2(2000, 800), 1f, new Color(0.212f, 0.086f, 0.129f)));
            playerField.Add(new TransformComponent(new Vector2(680, 384) -distFromCentre, (float)Math.PI * 0.33f));

            float spinSpeed = 0.002f;

            var playerDrum = w.Create();
            playerDrum.Add(new TextureComponent("Drum"));
            playerDrum.Add(new SpinComponent(spinSpeed));
            playerDrum.Add(new TransformComponent(new Vector2(36.5f*2, 31.5f*6)));
            playerDrum.Add(new DrawDataComponent(new Vector2(36.5f, 31.5f), new Vector2(4f), 1, Color.Black));

            for (int i = 0; i < context.BulletCount; i++) {
                var bullet = w.Create();
                bullet.Add(new TextureComponent("Bullet"));
                bullet.Add(new TransformComponent(36.5f * 4 + 140, 134 + 60 * i, MathHelper.PiOver2));
                bullet.Add(new DrawDataComponent(new Vector2(6, 20), Vector2.One * 4f, 1f, Color.Black));
                bullet.Add(new ChangeTintComponent(Color.White, 1f));

                bullet.Add(new AnimationComponent() {
                    OnEndAction = () =>
                    {
                       
                    }
                });
                if (i == 0)
                    bullet.Get<AnimationComponent>().Action = () =>
                    {
                        if (Input.PressedKey(Keys.Space)) {
                            SoundEngine.PlaySound("GunShot");
                            bullet.Add(new VelocityComponent(64, 0));
                        }
                    };

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
                enemies.Add(bug);
            }


            
            var blackBar = w.Create();
            blackBar.Add(new TextureComponent("Pixel"));
            blackBar.Add(new DrawDataComponent(Vector2.Zero, new Vector2(1360, 60), 1, Color.Black));
            blackBar.Add(new TransformComponent());

            var blackBar2 = w.Create();
            blackBar2.Add(new TextureComponent("Pixel"));
            blackBar2.Add(new DrawDataComponent(Vector2.Zero, new Vector2(1360, 60), 1, Color.Black));
            blackBar2.Add(new TransformComponent(0, 708));
            
            var crosshair = w.Create();
            crosshair.Add(new TextureComponent("Crosshair"));
            crosshair.Add(new DrawDataComponent(new Vector2(20), Vector2.One * 32f, 0f, Color.Crimson));
            crosshair.Add(new TransformComponent(1000, 240));
            crosshair.Add(new SpinComponent(0.006f));
            crosshair.Add(new OpacityComponent(0, 1.0f, 0.00f));
            crosshair.Add(new AnimationComponent() {
              Action = () => {
                  ref var scale = ref crosshair.Get<DrawDataComponent>();
                  ref var opacity = ref crosshair.Get<OpacityComponent>();
                  ref var position = ref crosshair.Get<TransformComponent>();
                  if (Input.PressedKey(Keys.Up)) {
                      if (++crosshairTarget > enemies.Count-1)
                          crosshairTarget = 0;
                  }

                  if (Input.PressedKey(Keys.Down)) {
                      if (--crosshairTarget < 0)
                          crosshairTarget = enemies.Count - 1;
                  }

                  if(enemies.Count > 0)
                    position.Position = enemies[crosshairTarget].Get<TransformComponent>().Position;

                  if (w.ActiveEntities.Any(x => x.Has<VelocityComponent>())/* || enemies.Count <= 0*/) {
                      if (opacity.CurrentValue > 0f)
                          opacity.CurrentValue -= 0.05f;

                      if (scale.Scale.X < 32f || scale.Scale.Y < 32f)
                          scale.Scale *= 1.04f;

                  } else {
                      if (opacity.CurrentValue < 1f)
                          opacity.CurrentValue += 0.06f;

                      if (scale.Scale.X > 6f || scale.Scale.Y > 6f)
                          scale.Scale *= 0.96f;
                  }
            }
            });
        }
    }
}
