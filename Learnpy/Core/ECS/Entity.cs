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

        public ref T GetComponent<T>()
            where T : struct
        {
            if (!BelongsTo.HasComponentCollection<T>())
                BelongsTo.AddCollection<T>();

            return ref BelongsTo.GetComponent<T>(Id);
        }

        public bool HasComponent<T>()
            where T : struct
            => BelongsTo.HasComponentCollection<T>() && BelongsTo.Components[typeof(T)].HasComponent(Id);
        public void AddComponent<T>(T component)
            where T : struct
        {
            if (!BelongsTo.HasComponentCollection<T>())
                BelongsTo.AddCollection<T>();

            BelongsTo.AddComponent(this.Id, component);
        }

        public void RemoveComponent<T>() where T : struct
        {
            if (!BelongsTo.HasComponentCollection<T>())
                BelongsTo.AddCollection<T>();

            BelongsTo.RemoveComponent(Id, GetComponent<T>());
        }
    }
}
