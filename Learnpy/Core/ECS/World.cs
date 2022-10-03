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

            // TO-DO: stuff
            Components.Add(typeof(TransformComponent), new ComponentCollection<TransformComponent>());
            Components.Add(typeof(TextureComponent), new ComponentCollection<TextureComponent>());
            Components.Add(typeof(BoxComponent), new ComponentCollection<BoxComponent>());
            Components.Add(typeof(PuzzleComponent), new ComponentCollection<PuzzleComponent>());
            Components.Add(typeof(MoveableComponent), new ComponentCollection<MoveableComponent>());
            Components.Add(typeof(DragComponent), new ComponentCollection<DragComponent>());

            Systems.Add(new CollisionSystem());

            Systems.Add(new DragSystem());

            Systems.Add(new ConnectionSystem());

            Systems.Add(new DrawSystem());

            Systems.Add(new RunCodeSystem());
            Systems.Add(new CompletionSystem());
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
            Type componentType = component.GetType();

            ComponentCollection<T> s = Components[componentType] as ComponentCollection<T>;

            s.Add(entityId, component);
        }

        public void RemoveComponent<T>(int entityId, T component)
            where T: struct
        {
            Components[component.GetType()].RemoveComponent(entityId);
        }

        public void RemoveComponent<T>(int entityId) where T : struct
        {
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
    }
}
