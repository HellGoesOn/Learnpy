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
                var puzzle = entity.GetComponent<PuzzleComponent>();

                if (Input.LMBReleased && puzzle.BeingDragged)
                {
                    entity.SetComponent(new PuzzleComponent(puzzle.PieceType, false) { StoredText = puzzle.StoredText });
                    var myBox = entity.GetComponent<BoxComponent>().Box;

                    foreach (int e2 in gameState.EntitiesById)
                    {
                        if (e2 == e)
                            continue;

                        var entity2 = gameState.Entities[e2];
                        var pos2 = entity2.GetComponent<TransformComponent>().Position;
                        var otherPuzzle = entity2.GetComponent<PuzzleComponent>();
                        var otherBox = entity2.GetComponent<BoxComponent>().Box;

                        if (Collision.BoundingBox(myBox, otherBox).Overlapped && otherPuzzle.CanBeConnectedTo)
                        {
                            if ((puzzle.PieceType == PieceType.Middle || puzzle.PieceType == PieceType.End) && (otherPuzzle.PieceType == PieceType.Beginning || otherPuzzle.PieceType == PieceType.Middle))
                            {
                                entity.SetComponent(new TransformComponent(pos2 + new Vector2(96, 0)));
                                entity.SetComponent(new PuzzleComponent(puzzle.PieceType) { StoredText = puzzle.StoredText, CanConnect = false, ConnectedTo = entity2.Id, ConnectionTo = puzzle.ConnectionTo });
                                entity2.SetComponent(new PuzzleComponent(otherPuzzle.PieceType) { StoredText = otherPuzzle.StoredText, CanBeConnectedTo = false, ConnectionTo = entity.Id, ConnectedTo = otherPuzzle.ConnectedTo});
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
