using Learnpy.Core.ECS;

namespace Learnpy.Content.Components
{
    public struct TextureComponent : IComponent
    {
        public readonly string Name;

        public TextureComponent(string texture)
        {
            Name = texture;
        }
    }
}
