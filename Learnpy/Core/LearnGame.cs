using Learnpy.Content;
using Learnpy.Content.Components;
using Learnpy.Content.Systems;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Learnpy.Collision;

namespace Learnpy.Core
{
    public class LearnGame : Game
    {
        internal World MainMenu;

		internal World MainWorld;

        internal Dictionary<GameState, World> Worlds = new Dictionary<GameState, World>();

        internal SpriteBatch spriteBatch;

		internal GraphicsDeviceManager gdm;

        internal GameState GameState;

        internal LearnGame()
        {
			this.IsMouseVisible = true;
            gdm = new GraphicsDeviceManager(this);
        }

		protected override void Initialize()
        {
            base.Initialize();

            gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
            gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
            gdm.ApplyChanges();

            MainWorld = new World();
            MainMenu = new World();

            GameState = GameState.MainMenu;

            Worlds.Add(GameState.MainMenu, MainMenu);
            Worlds.Add(GameState.Playground, MainWorld);

            // init main menu

            MainMenu.AddSystem<MenuSystem>();
            MainMenu.AddCollection<MenuComponent>();
            MainMenu.AddCollection<TransformComponent>();
            var mainMenu = MainMenu.Create();
            mainMenu.AddComponent(new TransformComponent());
            mainMenu.AddComponent(new MenuComponent
                (new MenuOption("Start", () =>
               {
                   GameState = GameState.Playground;
               }),
                new MenuOption("Options", () =>
                {
                    mainMenu.GetComponent<MenuComponent>().IsSelected = false;
                    mainMenu.GetComponent<TransformComponent>().Position = new Vector2(-150, 0);
                    var options = MainMenu.Create();
                var resolution = new MenuOption("Resolution", () =>
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
                    options.AddComponent(new MenuComponent(resolution,
                        new MenuOption("Back", () => 
                        {
                            MainMenu.Destroy(options.Id);
                            mainMenu.GetComponent<MenuComponent>().IsSelected = true;
                            mainMenu.GetComponent<TransformComponent>().Position = new Vector2(0, 0);
                        })) { IsSelected = true }
                        );
                }),
                new MenuOption("Quit", () =>
                {
                    this.Exit();
                })) {
                IsSelected = true
            });


            // init main playground
            MainWorld.AddCollection<TransformComponent>();
            MainWorld.AddCollection<TextureComponent>();
            MainWorld.AddCollection<BoxComponent>();
            MainWorld.AddCollection<PuzzleComponent>();
            MainWorld.AddCollection<MoveableComponent>();
            MainWorld.AddCollection<DragComponent>();

            MainWorld.AddSystem<CollisionSystem>();
            MainWorld.AddSystem<DragSystem>();
            MainWorld.AddSystem<ConnectionSystem>();
            MainWorld.AddSystem<DrawSystem>();
            MainWorld.AddSystem<RunCodeSystem>();
            MainWorld.AddSystem<CompletionSystem>();

            ResetWorld();

            SentenceFromText.Init();
            SentenceFromText.Load(MainWorld, 0);
        }

        internal void ResetWorld()
        {

            foreach (Entity entity in MainWorld.Entities)
            {
                if(entity.BelongsTo == null) 
                    continue;

                MainWorld.Destroy(entity.Id);
            }

            Random rand = new Random();

            for (int i = 0; i < 13; i++)
            {
                var e = MainWorld.Create();
                var pos = new Vector2(0, 56 * i);
                e.AddComponent(new TransformComponent(pos));
                e.AddComponent(new TextureComponent("PuzzlePiece"));
                e.AddComponent(new BoxComponent(new AABB(pos + new Vector2(64, 32), new Vector2(64, 32))));
                e.AddComponent(new PuzzleComponent(PieceType.Beginning));
            }

            CompletionSystem sys = MainWorld.GetSystem<CompletionSystem>();
            sys.Time = 0;
            sys.Succesful = false;
        }

        protected override void LoadContent()
		{
			base.LoadContent();

			Assets.LoadAssets();

			spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
			Assets.Unload();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

            if(GameOptions.NeedsUpdate) {
                gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
                gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
                gdm.ApplyChanges();
                GameOptions.NeedsUpdate = false;
            }

            if (GameState == GameState.Playground && Input.PressedKey(Keys.C))
            {
                ResetWorld();
                SentenceFromText.Load(MainWorld, MainWorld.GetSystem<CompletionSystem>().LevelTarget-1);
            }

            if(Input.PressedKey(Keys.D1)) {
                GameState = GameState.Playground;
            }

			Worlds[GameState].Update();

			Input.Update();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			base.Draw(gameTime);

			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
			Worlds[GameState].Draw(this);
			spriteBatch.End();
		}
	}
}
