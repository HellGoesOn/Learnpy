﻿using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;

namespace Learnpy.Content.Components
{
    public struct TransformComponent
    {
        public Vector2 Position;

        public TransformComponent(Vector2 position)
        {
            Position = position;
        }
    }
}