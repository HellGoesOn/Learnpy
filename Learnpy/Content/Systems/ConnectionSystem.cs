using Learnpy.Content.Components;
using Learnpy.Content.Scenes;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Learnpy.Content.Systems
{
    public class ConnectionSystem : ISystem
    {
        public void Execute(World gameState)
        {
            foreach (Entity entity in gameState.ActiveEntities.Where(x => x.Has<PuzzleComponent>() && x.Has<DragComponent>()))
            {
                ref var puzzle = ref entity.Get<PuzzleComponent>();
                ref var drag = ref entity.Get<DragComponent>();

                if (Input.LMBReleased && puzzle.BeingDragged && drag.Active)
                {
                    puzzle.BeingDragged = false;
                    drag.Active = false;

                    if(puzzle.ConnectionTo != -1)
                    {
                        int connect = puzzle.ConnectionTo;
                        while (connect != -1)
                        {
                            Entity connectedChild = gameState.Entities[connect];
                            connectedChild.Get<DragComponent>().Active = false;
                            connect = connectedChild.Get<PuzzleComponent>().ConnectionTo;
                        }
                    }

                    var myBox = entity.Get<BoxComponent>().Box;

                    foreach (int e2 in gameState.EntitiesById)
                    {
                        if (e2 == entity.Id || e2 == puzzle.ConnectionTo)
                            continue;

                        var entity2 = gameState.Entities[e2];
                        var pos2 = entity2.Get<TransformComponent>().Position;
                        ref var otherPuzzle = ref entity2.Get<PuzzleComponent>();
                        var otherBox = entity2.Get<BoxComponent>().Box;

                        if (Collision.BoundingBox(myBox, otherBox).Overlapped && otherPuzzle.CanBeConnectedTo)
                        {
                            if ((puzzle.PieceType == PieceType.Middle || puzzle.PieceType == PieceType.End) && (otherPuzzle.PieceType == PieceType.Beginning || otherPuzzle.PieceType == PieceType.Middle))
                            {
                                ref var trsf = ref entity.Get<TransformComponent>();
                                trsf.Position = pos2 + new Vector2(114, 0);

                                int connect = puzzle.ConnectionTo;
                                int parent = entity.Id;
                                while (connect != -1)
                                {
                                    var parentPos = gameState.Entities[parent].Get<TransformComponent>().Position;
                                    Entity connectedChild = gameState.Entities[connect];
                                    connectedChild.Get<TransformComponent>().Position = parentPos + new Vector2(114, 0);
                                    parent = connectedChild.Id;
                                    connect = connectedChild.Get<PuzzleComponent>().ConnectionTo;
                                }

                                puzzle.CanConnect = false;
                                puzzle.ConnectedTo = entity2.Id;
                                otherPuzzle.CanBeConnectedTo = false;
                                otherPuzzle.ConnectionTo = entity.Id;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Render(LearnGame gameRenderer)
        {
        }
    }
}
