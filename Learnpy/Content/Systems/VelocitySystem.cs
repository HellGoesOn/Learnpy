using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.ECS;
using System.Linq;

namespace Learnpy.Content.Systems
{
    public class VelocitySystem : ISystem
    {
        public void Execute(World gameState)
        {
            foreach(Entity e in gameState.ActiveEntities.Where(x => x.HasComponent<TransformComponent>() && x.HasComponent<VelocityComponent>())) {

                ref var transform = ref e.GetComponent<TransformComponent>();
                ref var velocity = ref e.GetComponent<VelocityComponent>();

                transform.Position += velocity.Value;
            }
        }

        public void Render(LearnGame gameRenderer)
        {
        }
    }
}
