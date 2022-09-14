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

        public Entity[] Entities;
        public Dictionary<Type, IComponent[]> Components;
        public List<ISystem> Systems;

        public World()
        {
            Entities = new Entity[Globals.MAX_ENTITY_COUNT];
            Components = new Dictionary<Type, IComponent[]>();
            Systems = new List<ISystem>();

            for (int i = 0; i < Globals.MAX_ENTITY_COUNT; i++)
            {
                IDQueue.Add(i);
            }

            // TO-DO: stuff
            Components.Add(typeof(TransformComponent), new IComponent[Globals.MAX_ENTITY_COUNT]);
            Components.Add(typeof(TextureComponent), new IComponent[Globals.MAX_ENTITY_COUNT]);
            Components.Add(typeof(BoxComponent), new IComponent[Globals.MAX_ENTITY_COUNT]);
            Components.Add(typeof(PuzzleComponent), new IComponent[Globals.MAX_ENTITY_COUNT]);
            Components.Add(typeof(MoveableComponent), new IComponent[Globals.MAX_ENTITY_COUNT]);

            Systems.Add(new CollisionSystem());

            Systems.Add(new DragSystem());

            Systems.Add(new ConnectionSystem());

            Systems.Add(new DrawSystem());

            Systems.Add(new RunCodeSystem());
        }

        public void Update()
        {
            DateTime beginning = DateTime.Now;
            Console.WriteLine($"Update Loop started at {beginning}");
            foreach (ISystem system in Systems)
            {
                system.Execute(this);
            }
            Console.WriteLine($"Took: {(DateTime.Now - beginning).Milliseconds}ms");
        }

        public void Draw(LearnGame game)
        {
            DateTime beginning = DateTime.Now;
            Console.WriteLine($"Draw Loop started at {beginning}");
            foreach (ISystem sys in Systems)
            {
                sys.Render(game);
            }
            Console.WriteLine($"Took: {(DateTime.Now - beginning).Milliseconds}ms");
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

            return e;
        }

        public void Destroy(int entityId)
        {
            Entities[entityId] = default;
            IDQueue.Insert(0, entityId);
            EntitiesById.Remove(entityId);
            LogWorldData();
        }

        public void AddComponent(int entityId, IComponent component)
        {
            Type componentType = component.GetType();
            Components[componentType][entityId] = component;

            Console.WriteLine($"Added {component.GetType().Name} to Entity {entityId}");
            LogWorldData();
        }

        public void RemoveComponent(int entityId, IComponent component)
        {
            Components[component.GetType()][entityId] = default;
            Console.WriteLine($"Removed {component.GetType().Name} from Entity {entityId}");
            LogWorldData();
        }

        public void RemoveComponent<T>(int entityId) where T : IComponent
        {
            Components[typeof(T)][entityId] = default;
            Console.WriteLine($"Removed {typeof(T).Name} from Entity {entityId}");
            LogWorldData();
        }

        public T GetComponent<T>(int entityId) where T : IComponent
        {
            var component = (T)Components[typeof(T)][entityId];
            return component;
        }

        public void LogWorldData()
        {
            Console.WriteLine($"Current Entity Count is {EntitiesById.Count}");
            Console.WriteLine($"Current Free Entity ID Count is {IDQueue.Count}");
        }
    }
}
