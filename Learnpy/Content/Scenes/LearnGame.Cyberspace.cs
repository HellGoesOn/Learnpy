using Learnpy.Content.Components;
using Learnpy.Content.Scenes.Transitions;
using Learnpy.Content.Systems;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Scenes
{
    public partial class LearnGame : Game
    {
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

            options.Add(new OpacityComponent(0f, 0f, 0.1f));
                })
                {LocalePath ="monologue.txt"},
                new MenuOption("try", () =>
                {
                    sceneTransitions.Add(new FadeToBlack(GameState, GameState.Playground));
                })
                {LocalePath ="monologue.txt"}
            }) {
                IsSelected = true
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
            }) { AutoScroll = true, Speed = 4f, CenteredOrigin = true, Color = Color.Wheat, TimeUntilNextPageMax = 180,
            OnDialogueEnd = () =>
            {
                player.Add(new TransformComponent(680, 384));
                player.Add(new OpacityComponent(0.0f, 1.0f, 0.1f));
                options.Add(new OpacityComponent(0f, 1f, 0.1f));
                diag.Add(new OpacityComponent(1f, 0.0f, 0.05f)); 
            }
        });
            diag.Add(new TransformComponent(new Vector2(680, 580)));
        }
    }
}
