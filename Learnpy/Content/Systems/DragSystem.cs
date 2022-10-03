using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
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

            foreach (Entity entity in gameState.ActiveEntities.Where(x => x.HasComponent<PuzzleComponent>() && x.HasComponent<MoveableComponent>() && x.HasComponent<DragComponent>()))
            {
                ref var puzzle = ref entity.GetComponent<PuzzleComponent>();
                var entityBox = entity.GetComponent<BoxComponent>();
                ref var oldPos = ref entity.GetComponent<TransformComponent>();
                ref DragComponent drag = ref entity.GetComponent<DragComponent>();

                if (drag.Active)
                {
                    oldPos.Position = Input.MousePos - drag.DragOffset - entityBox.Box.Halfwidths;
                }

                if (Input.LMBClicked && BoundingBox(entityBox.Box, Input.MouseBox).Overlapped)
                {
                    if (puzzle.ConnectedTo != -1)
                    {
                        var otherEntity = gameState.Entities[puzzle.ConnectedTo];
                        ref var puz = ref otherEntity.GetComponent<PuzzleComponent>();
                        puz.ConnectionTo = -1;
                        puz.CanConnect = true;
                        puz.CanBeConnectedTo = true;
                    }

                    puzzle.BeingDragged = true;
                    puzzle.CanConnect = true;
                    puzzle.CanBeConnectedTo = true;
                    puzzle.ConnectionTo = -1;
                    puzzle.ConnectedTo = -1;

                    drag.Active = true;
                    drag.DragOffset = Input.MousePos - entityBox.Box.Centre;

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
