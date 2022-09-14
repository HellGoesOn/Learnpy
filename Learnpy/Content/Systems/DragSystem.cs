using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static Learnpy.Collision;

namespace Learnpy.Content.Systems
{
    public class DragSystem : ISystem
    {
        // Violation of ECS
        public bool Show;

        public void Execute(World gameState)
        {
            if (Input.PressedKey(Keys.H))
                Show = !Show;

            foreach (int e in gameState.EntitiesById)
            {
                var entity = gameState.Entities[e];
                var puzzle = entity.GetComponent<PuzzleComponent>();
                var entityBox = entity.GetComponent<BoxComponent>();
                var oldPos = entity.GetComponent<TransformComponent>().Position;

                if (Input.LMBHeld && BoundingBox(entityBox.Box, Input.MouseBox).Overlapped && entity.HasComponent<MoveableComponent>())
                {
                    if (puzzle.ConnectedTo != -1)
                    {
                        var otherEntity = gameState.Entities[puzzle.ConnectedTo];
                        var puz = otherEntity.GetComponent<PuzzleComponent>();
                        otherEntity.SetComponent(new PuzzleComponent(puz.PieceType) { StoredText = puz.StoredText });
                    }

                    entity.SetComponent(new PuzzleComponent(puzzle.PieceType, true) { StoredText = puzzle.StoredText});
                    entity.SetComponent(new TransformComponent(oldPos + Input.PositionDifference));
                    entity.SetComponent(new BoxComponent(oldPos + Input.PositionDifference+entityBox.Box.Halfwidths, entityBox.Box.Halfwidths));
                }

            }
        }

        public void Render(LearnGame gameRenderer)
        {
            if (!Show)
                return;

            AABB mouseBox = Input.MouseBox;
            Util.DrawRectangle(gameRenderer.spriteBatch, mouseBox.Centre - mouseBox.Halfwidths, mouseBox.Halfwidths * 2, Color.Red, 1);
            foreach (int e in gameRenderer.MainWorld.EntitiesById)
            {
                var entity = gameRenderer.MainWorld.Entities[e];
                var entityBox = entity.GetComponent<BoxComponent>().Box;
                Util.DrawRectangle(gameRenderer.spriteBatch, entityBox.Centre - entityBox.Halfwidths, entityBox.Halfwidths * 2, Color.Green * 0.3f, .9f);
            }
        }
    }
}
