using Learnpy.Core.ECS;

namespace Learnpy.Content.Components
{
    public struct MoveableComponent : IComponent
    {
        public bool CanMove;

        public MoveableComponent(bool canMove)
        {
            CanMove = canMove;
        }
    }
}
