using System;

namespace Learnpy.Core.ECS
{
    public struct Entity
    {
        public readonly int Id;

        public readonly World BelongsTo;

        public Entity(World world, int id = -1)
        {
            Id = id;
            BelongsTo = world;
        }

        public T GetComponent<T>()
            where T : IComponent
        {
            return (T)BelongsTo.Components[typeof(T)][Id];
        }

        public void SetComponent(IComponent component)
        {
            BelongsTo.Components[component.GetType()][Id] = component;
        }

        public bool HasComponent<T>()
            where T : IComponent
        {
            return BelongsTo.Components[typeof(T)][Id] != default;
        }

        public void AddComponent(IComponent component)
        {
            BelongsTo.AddComponent(this.Id, component);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            BelongsTo.RemoveComponent(Id, GetComponent<T>());
        }
    }
}
