using Learnpy.Content.Components;
using Learnpy.Content.Scenes;
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

            MouseResult mouse = Input.ScreenToWorldSpace(gameState);

            foreach (Entity entity in gameState.ActiveEntities.Where(x => x.Has<PuzzleComponent>() && x.Has<MoveableComponent>() && x.Has<DragComponent>()))
            {
                ref var puzzle = ref entity.Get<PuzzleComponent>();
                var entityBox = entity.Get<BoxComponent>();
                ref var oldPos = ref entity.Get<TransformComponent>();
                ref DragComponent drag = ref entity.Get<DragComponent>();

                if (drag.Active)
                {
                    oldPos.Position = mouse.Position - drag.DragOffset - entityBox.Box.Halfwidths;
                }

                if (Input.LMBClicked && BoundingBox(entityBox.Box, mouse.Bounds).Overlapped)
                {
                    if(puzzle.ConnectedTo != -1)
                    {
                        var otherEntity = gameState.Entities[puzzle.ConnectedTo];
                        BoxComponent otherBox = otherEntity.Get<BoxComponent>();
                        ref var puz = ref otherEntity.Get<PuzzleComponent>();
                        ref var otherDraggalbe = ref otherEntity.Get<DragComponent>();

                        puz.ConnectionTo = -1;
                        puz.CanBeConnectedTo = true;
                    }

                    if (puzzle.ConnectionTo != -1)
                    {
                        int connect = puzzle.ConnectionTo;
                        while (connect != -1)
                        {
                            var otherEntity = gameState.Entities[connect];
                            BoxComponent otherBox = otherEntity.Get<BoxComponent>();
                            ref var puz = ref otherEntity.Get<PuzzleComponent>();
                            ref var otherDraggalbe = ref otherEntity.Get<DragComponent>();
                            otherDraggalbe.Active = true;
                            otherDraggalbe.DragOffset = Input.ScaledMousePos - otherBox.Box.Centre;

                            connect = puz.ConnectionTo;
                        }
                    }

                    puzzle.BeingDragged = true;
                    puzzle.CanConnect = true;
                    //puzzle.CanBeConnectedTo = true;
                    //puzzle.ConnectionTo = -1;
                    puzzle.ConnectedTo = -1;

                    drag.Active = true;
                    drag.DragOffset = mouse.Position - entityBox.Box.Centre;

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
                var entityBox = entity.Get<BoxComponent>().Box;
                Util.DrawRectangle(gameRenderer.spriteBatch, entityBox.Centre - entityBox.Halfwidths, entityBox.Halfwidths * 2, Color.Green * 0.3f, .9f);
            }
        }
    }
}
