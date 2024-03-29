﻿using Learnpy.Content;
using Learnpy.Content.Components;
using Learnpy.Content.Scenes.Transitions;
using Learnpy.Content.Systems;
using Learnpy.Core;
using Learnpy.Core.Drawing;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Learnpy.Collision;

namespace Learnpy.Content.Scenes
{
    public partial class LearnGame : Game
    {

        internal World MainMenu;

		internal World MainWorld;

        internal Dictionary<GameState, World> Worlds = new Dictionary<GameState, World>();

        internal SpriteBatch spriteBatch;

		internal GraphicsDeviceManager gdm;

        internal GameState GameState;

        internal int GameWidth = 1360, GameHeight = 768;

        internal Vector2 Size => new Vector2(GameWidth, GameHeight);

        internal List<ISceneTransition> sceneTransitions = new List<ISceneTransition>();

        internal float deltaTime;

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
            Renderer.MainTarget = new RenderTarget2D(gdm.GraphicsDevice, 1360, 768);
            gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
            gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
            gdm.ApplyChanges();

            Worlds.Add(GameState.LoginScreen, new());
            Worlds.Add(GameState.CombatSelect, new());

            Worlds.Add(GameState.Cyberspace, new World());
            MainWorld = new World();
            MainWorld.camera.centre = new Vector2(1360, 768) * 0.5f;
            MainMenu = new World();
            MainMenu.camera.centre = new Vector2(1360, 768) * 0.5f;
            Worlds.Add(GameState.Combat, new World());
            Worlds[GameState.Combat].camera.centre = new Vector2(1360, 768) * 0.5f;
            Worlds[GameState.CombatSelect].camera.centre = new Vector2(1360, 768) * 0.5f;

            GameState = GameState.LoginScreen;

            Worlds.Add(GameState.MainMenu, MainMenu);
            Worlds.Add(GameState.Playground, MainWorld);

            // init main menu

            InitializeMainMenu();
            InitializeCyberspace();
            BeginCombat(null);
            DoLogin();

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
                e.Add(new TransformComponent(pos));
                e.Add(new TextureComponent("PuzzlePiece"));
                e.Add(new BoxComponent(new AABB(pos + new Vector2(64, 32), new Vector2(64, 32))));
                e.Add(new PuzzleComponent(PieceType.Beginning));
                e.Add(new AnimationComponent(new[] { new Rectangle(0, 0, 128, 64) }));
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
            deltaTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            SoundEngine.Update();
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

            if (Input.PressedKey(Keys.F4)) 
                sceneTransitions.Add(new SlideTransition(GameState, GameState.MainMenu, (Direction)new Random().Next((int)Direction.Down + 1)) {
                Color = Color.Black,
                SlideSpeed = 0.02f
            });

            foreach (ISceneTransition transition in sceneTransitions) {
                transition.Update(this);

                if (transition.SceneChanged())
                    transition.OnSceneChanged()?.Invoke();

                if (transition.IsFinished())
                    transition.OnTransitionEnd()?.Invoke();
            }

			Worlds[GameState].Update();

            sceneTransitions.RemoveAll(x => x.IsFinished());
			Input.Update();
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

            if(sceneTransitions.Count <= 0)
                Renderer.DrawScene(Worlds[GameState], Worlds[GameState].camera, Renderer.MainTarget);
            else {
                foreach(ISceneTransition transition in sceneTransitions) {
                    transition.Draw(this);
                }
            }
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(Renderer.MainTarget, new Rectangle(0, 0, GameOptions.ScreenWidth, GameOptions.ScreenHeight), Color.White);
            /*Renderer.DrawText($"AnyVelocity:{Worlds[GameState].ActiveEntities.FirstOrDefault(x =>x.Has<VelocityComponent>()).Id};Entities: {Worlds[GameState].ActiveEntities.Count}; MousePos: {Input.ScaledMousePos};" +
                $" ToScreen:{Input.ScreenToWorldSpace(Worlds[GameState]).Position}" +
                $" CamPos:{Worlds[GameState].camera.centre}", new Vector2(40), Assets.DefaultFont, Color.Lime, 0f, Vector2.One, Vector2.Zero, SpriteEffects.None);*/
            spriteBatch.End();
		}
	}
}
