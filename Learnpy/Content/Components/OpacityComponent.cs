using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Components
{
    public struct OpacityComponent
    {
        public float CurrentValue;
        public float TargetValue;
        public float Speed;

        public OpacityComponent(float current, float target, float speed)
        {
            CurrentValue = current;
            TargetValue = target;
            Speed = speed;
        }

    }
}
