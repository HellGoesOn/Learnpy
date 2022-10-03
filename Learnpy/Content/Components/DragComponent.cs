using Microsoft.Xna.Framework;

namespace Learnpy.Content.Components
{
    public struct DragComponent
    {
        public bool Active { get; set; }

        public Vector2 DragOffset { get; set; }
    }
}
