using Learnpy.Content.Components;
using Learnpy.Content.Scenes;
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
                ref var entityBox = ref entity.Get<BoxComponent>();
                var pos = entity.Get<TransformComponent>().Position;

                entityBox.Box = new Collision.AABB(pos + entityBox.Box.Halfwidths, entityBox.Box.Halfwidths);
            }
        }

        public void Render(LearnGame gameRenderer)
        {
        }
    }
}
