using Microsoft.Xna.Framework;
using System;

namespace Learnpy.Core.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 RotateBy(this Vector2 v, float rotation)
        {
            float xx = v.X * (float)Math.Cos(rotation) - v.Y * (float)Math.Sin(rotation);
            float yy = v.X * (float)Math.Sin(rotation) + v.Y * (float)Math.Cos(rotation);

            return new Vector2(xx, yy);
        }
    }
}
