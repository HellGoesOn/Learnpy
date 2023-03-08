using Learnpy.Content.Components;
using Learnpy.Content.Systems;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Learnpy.Core.ECS
{
    public class World
    {
        public List<int> IDQueue = new List<int>();
        public List<int> EntitiesById = new List<int>();
        public List<Entity> ActiveEntities = new List<Entity>();

        public Entity[] Entities;
        public Dictionary<Type, IComponentCollection> Components;
        public List<ISystem> Systems;

        public World()
        {
            Entities = new Entity[Globals.MAX_ENTITY_COUNT];
            Components = new Dictionary<Type, IComponentCollection>();
            Systems = new List<ISystem>();

            for (int i = 0; i < Globals.MAX_ENTITY_COUNT; i++)
            {
                IDQueue.Add(i);
            }
        }

        public void Update()
        {
            foreach (ISystem system in Systems)
            {
                system.Execute(this);
            }
        }

        public void Draw(LearnGame game)
        {
            foreach (ISystem sys in Systems)
            {
                sys.Render(game);
            }
        }

        public Entity Create()
        {
            if (IDQueue.Count <= 0)
                throw new System.Exception("Entity limit exceeded. Connection Terminated.");

            int id = IDQueue[0];

            Entity e = new Entity(this, id);

            Entities[id] = e;

            IDQueue.RemoveAt(0);
            EntitiesById.Add(id);
            ActiveEntities.Add(e);

            return e;
        }

        public void Destroy(int entityId)
        {
            foreach (var comp in Components.Keys)
            {
                Components[comp].RemoveComponent(entityId);
            }
            ActiveEntities.Remove(Entities[entityId]);
            IDQueue.Insert(0, entityId);
            EntitiesById.Remove(entityId);
            Entities[entityId] = default;
        }

        public void AddComponent<T>(int entityId, T component)
            where T : struct
        {
            if (!HasComponentCollection<T>())
                return;

            Type componentType = component.GetType();

            ComponentCollection<T> s = Components[componentType] as ComponentCollection<T>;

            s.Add(entityId, component);
        }

        public void RemoveComponent<T>(int entityId, T component)
            where T: struct
        {
            if (!HasComponentCollection<T>())
                return;

            Components[component.GetType()].RemoveComponent(entityId);
        }

        public void RemoveComponent<T>(int entityId) where T : struct
        {
            if (!HasComponentCollection<T>())
                return;

            Components[typeof(T)].RemoveComponent(entityId);
        }

        public ref T GetComponent<T>(int entityId) where T : struct
            => ref (Components[typeof(T)] as ComponentCollection<T>).Get(entityId);

        public T GetSystem<T>()
            where T : ISystem
        {
            if (Systems.Count <= 0 || Systems.Find(x => x.GetType() == typeof(T)) == null)
                return default(T);

            return (T)Systems.Find(x => x.GetType() == typeof(T));
        }

        public bool HasComponentCollection<T>() where T : struct => Components.ContainsKey(typeof(T));

        public void AddCollection<T>() where T : struct
        {
            Components.Add(typeof(T), new ComponentCollection<T>());
        }

        public void AddSystem<T>() where T : ISystem, new()
        {
            Systems.Add(new T());
        }
    }
}
