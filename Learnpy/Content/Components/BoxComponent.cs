using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using static Learnpy.Collision;

namespace Learnpy.Content.Components
{
    public struct BoxComponent : IComponent
    {
        public AABB Box;

        public BoxComponent(AABB box)
        {
            Box = box;
        }

        public BoxComponent(Vector2 centre, Vector2 halfWidths)
        {
            Box = new AABB(centre, halfWidths);
        }

    }
}
