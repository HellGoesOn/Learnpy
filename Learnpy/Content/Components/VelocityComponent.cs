using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Components
{
    public struct VelocityComponent
    {
        public Vector2 Value;

        public VelocityComponent(Vector2 velocity)
        {
            Value = velocity;
        }

        public VelocityComponent(float x, float y)
        {
            this.Value = new Vector2(x, y);
        }
    }
}
