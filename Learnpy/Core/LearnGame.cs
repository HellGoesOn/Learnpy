using Learnpy.Content;
using Learnpy.Content.Components;
using Learnpy.Content.Systems;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static Learnpy.Collision;
using static Learnpy.Content.Enums;

namespace Learnpy.Core
{
    public class LearnGame : Game
    {
		internal World MainWorld;

		internal SpriteBatch spriteBatch;

		internal GraphicsDeviceManager gdm;

        internal LearnGame()
        {
			this.IsMouseVisible = true;
            gdm = new GraphicsDeviceManager(this);
        }

		protected override void Initialize()
        {
            base.Initialize();

            gdm.PreferredBackBufferWidth = 1366;
            gdm.PreferredBackBufferHeight = 768;
            gdm.ApplyChanges();

            MainWorld = new World();

            DateTime beginning = DateTime.Now;
            Console.WriteLine($"Started at {beginning}");
            ResetWorld();

            Console.WriteLine($"Took: {(DateTime.Now - beginning).Milliseconds}ms");

            SentenceFromText.Init();
            SentenceFromText.Load(MainWorld, 0);
        }

        internal void ResetWorld()
        {
            foreach(Entity entity in MainWorld.Entities)
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

            if (Input.PressedKey(Keys.C))
            {
                ResetWorld();
                SentenceFromText.Load(MainWorld, MainWorld.GetSystem<CompletionSystem>().LevelTarget-1);
            }
			MainWorld.Update();

			Input.Update();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			base.Draw(gameTime);

			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
			MainWorld.Draw(this);
			spriteBatch.End();
		}
	}
}
