using System;

namespace Learnpy.Core.ECS
{
    public struct Entity
    {
        public readonly int Id;

        public bool IsDeleted { get; set; }

        public readonly World BelongsTo;

        public Entity(World world, int id = -1)
        {
            IsDeleted = false;
            Id = id;
            BelongsTo = world;
        }

        public ref T Get<T>()
            where T : struct
        {
            if (!BelongsTo.HasComponentCollection<T>())
                BelongsTo.AddCollection<T>();

            return ref BelongsTo.GetComponent<T>(Id);
        }

        public bool Has<T>()
            where T : struct
            => BelongsTo.HasComponentCollection<T>() && BelongsTo.Components[typeof(T)].HasComponent(Id);

        public void Add<T>(T component)
            where T : struct
        {
            if (!BelongsTo.HasComponentCollection<T>())
                BelongsTo.AddCollection<T>();

            BelongsTo.AddComponent(this.Id, component);
        }

        public void Remove<T>() where T : struct
        {
            if (!BelongsTo.HasComponentCollection<T>())
                BelongsTo.AddCollection<T>();

            BelongsTo.RemoveComponent(Id, Get<T>());
        }
    }
}
