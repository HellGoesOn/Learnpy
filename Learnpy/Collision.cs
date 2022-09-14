using Microsoft.Xna.Framework;
using System;

namespace Learnpy
{
    public class Collision
    {
        public static CollisionResult BoundingBox(AABB left, AABB right)
        {
            bool a = Math.Abs(left.Centre.X - right.Centre.X) <= left.Halfwidths.X + right.Halfwidths.X;
            bool b = Math.Abs(left.Centre.Y - right.Centre.Y) <= left.Halfwidths.Y + right.Halfwidths.Y;

            return new CollisionResult(a && b)
            {
                Left = left,
                Right = right,
            };
        }

        public struct CollisionResult
        {
            public CollisionResult(bool overlap)
            {
                Overlapped = overlap;
                Left = new AABB();
                Right = new AABB();
            }

            public bool XAxisCollided
            {
                get
                {
                    bool xCollision = Right.Centre.X + Right.Halfwidths.X > Left.Centre.X - Left.Halfwidths.X
                        || Right.Centre.X - Right.Halfwidths.X < Left.Centre.X + Left.Halfwidths.X;

                    return xCollision && Overlapped;
                }
            }

            public bool YAxisCollided
            {
                get
                {
                    bool yCollision = Right.Centre.Y + Right.Halfwidths.Y > Left.Centre.Y - Left.Halfwidths.Y
                        || Right.Centre.Y - Right.Halfwidths.Y < Left.Centre.Y + Left.Halfwidths.Y;

                    return yCollision && Overlapped;
                }
            }

            public bool Overlapped { get; set; }

            public bool AnyCollision => XAxisCollided || YAxisCollided;

            public AABB Left { get; set; }

            public AABB Right { get; set; }
        }

        public struct AABB
        {
            public Vector2 Centre;
            public Vector2 Halfwidths;

            public AABB(Vector2 centre, Vector2 halfwidths)
            {
                Centre = centre;
                Halfwidths = halfwidths;
            }
        }
    }
}
