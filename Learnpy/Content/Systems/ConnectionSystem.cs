using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using static Learnpy.Content.Enums;

namespace Learnpy.Content.Systems
{
    public class ConnectionSystem : ISystem
    {
        public void Execute(World gameState)
        {
            foreach (int e in gameState.EntitiesById)
            {
                var entity = gameState.Entities[e];
                ref var puzzle = ref entity.GetComponent<PuzzleComponent>();

                if (Input.LMBReleased && puzzle.BeingDragged)
                {
                    puzzle.BeingDragged = false;
                    entity.GetComponent<DragComponent>().Active = false;

                    var myBox = entity.GetComponent<BoxComponent>().Box;

                    foreach (int e2 in gameState.EntitiesById)
                    {
                        if (e2 == e)
                            continue;

                        var entity2 = gameState.Entities[e2];
                        var pos2 = entity2.GetComponent<TransformComponent>().Position;
                        ref var otherPuzzle = ref entity2.GetComponent<PuzzleComponent>();
                        var otherBox = entity2.GetComponent<BoxComponent>().Box;

                        if (Collision.BoundingBox(myBox, otherBox).Overlapped && otherPuzzle.CanBeConnectedTo)
                        {
                            if ((puzzle.PieceType == PieceType.Middle || puzzle.PieceType == PieceType.End) && (otherPuzzle.PieceType == PieceType.Beginning || otherPuzzle.PieceType == PieceType.Middle))
                            {
                                ref var trsf = ref entity.GetComponent<TransformComponent>();
                                trsf.Position = pos2 + new Vector2(114, 0);
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
