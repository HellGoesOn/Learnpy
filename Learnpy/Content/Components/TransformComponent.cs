using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;

namespace Learnpy.Content.Components
{
    public struct TransformComponent
    {
        public Vector2 Position;
        public float Rotation;

        public TransformComponent(Vector2 position, float rotation = 0f)
        {
            Position = position;
            Rotation = rotation;
        }

        public TransformComponent(float x, float y, float rotation = 0f)
        {
            Position = new Vector2(x, y);
            Rotation = rotation;
        }
    }
}
