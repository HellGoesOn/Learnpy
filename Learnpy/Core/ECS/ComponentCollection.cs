namespace Learnpy.Core.ECS
{
    public interface IComponentCollection
    {
        object GetComponent(int id);
        bool HasComponent(int id);
        void RemoveComponent(int id);
    }

    public class ComponentCollection<T> : IComponentCollection
        where T : struct
    {
        private readonly T[] components = new T[Globals.MAX_ENTITY_COUNT];
        private readonly bool[] activeComponents = new bool[Globals.MAX_ENTITY_COUNT];

        public object GetComponent(int id) => Get(id);

        public ref T Get(int id) => ref components[id];

        public bool HasComponent(int id) => activeComponents[id];

        public void Add(int id, T component)
        {
            activeComponents[id] = true;
            components[id] = component;
        }

        public void RemoveComponent(int id)
        {
            activeComponents[id] = false;
            components[id] = default;
        }
    }
}
