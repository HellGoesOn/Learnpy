using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;

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
