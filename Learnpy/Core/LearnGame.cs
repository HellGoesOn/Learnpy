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

        internal RenderTarget2D RenderTarget;

        internal int GameWidth = 1360, GameHeight = 768;

        internal Vector2 Size => new Vector2(GameWidth, GameHeight);

        internal LearnGame()
        {
            GameOptions.Load();
            Locale.Fill();
			this.IsMouseVisible = true;
            gdm = new GraphicsDeviceManager(this);
        }

		protected override void Initialize()
        {
            base.Initialize();
            RenderTarget = new RenderTarget2D(gdm.GraphicsDevice, 1360, 768);
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

            for (int i = 0; i < 13; i++) {
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
            GameOptions.Save();
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
			base.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(Color.DarkSeaGreen);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
			Worlds[GameState].Draw(this);
			spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(RenderTarget, new Rectangle(0, 0, GameOptions.ScreenWidth, GameOptions.ScreenHeight), Color.White);
            spriteBatch.End();
		}
	}
}
