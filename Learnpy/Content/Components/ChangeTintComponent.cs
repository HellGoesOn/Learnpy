using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Components
{
    public struct ChangeTintComponent
    {
        public float Amount;
        public Color TargetTint;

        public ChangeTintComponent(Color targetColor, float speed)
        {
            TargetTint = targetColor;
            Amount = speed;
        }
    }
}
