using Learnpy.Content.Components;
using Learnpy.Content.Scenes;
using Learnpy.Core.ECS;
using System.Linq;

namespace Learnpy.Content.Systems
{
    public class VelocitySystem : ISystem
    {
        public void Execute(World gameState)
        {
            foreach(Entity e in gameState.ActiveEntities.Where(x => x.Has<TransformComponent>() && x.Has<VelocityComponent>())) {

                ref var transform = ref e.Get<TransformComponent>();
                ref var velocity = ref e.Get<VelocityComponent>();

                transform.Position += velocity.Value;
            }
        }

        public void Render(LearnGame gameRenderer)
        {
        }
    }
}
