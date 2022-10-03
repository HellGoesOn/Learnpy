using Learnpy.Core.ECS;

namespace Learnpy.Content.Components
{
    public struct TextureComponent
    {
        public readonly string Name;

        public TextureComponent(string texture)
        {
            Name = texture;
        }
    }
}
