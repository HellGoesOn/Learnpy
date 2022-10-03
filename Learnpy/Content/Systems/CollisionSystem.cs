using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.ECS;

namespace Learnpy.Content.Systems
{
    public class CollisionSystem : ISystem
    {
        public void Execute(World gameState)
        {
            foreach (int e in gameState.EntitiesById)
            {
                var entity = gameState.Entities[e];
                ref var entityBox = ref entity.GetComponent<BoxComponent>();
                var pos = entity.GetComponent<TransformComponent>().Position;

                entityBox.Box = new Collision.AABB(pos + entityBox.Box.Halfwidths, entityBox.Box.Halfwidths);
            }
        }

        public void Render(LearnGame gameRenderer)
        {
        }
    }
}
