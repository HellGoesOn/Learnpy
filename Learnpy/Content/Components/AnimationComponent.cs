using Microsoft.Xna.Framework;
using System;

namespace Learnpy.Content.Components
{
    public struct AnimationComponent
    {
        public int CurrentFrame;
        public Rectangle[] Frames;
        public Action Action { get; set; }
        public Action OnEndAction { get; set; }

        public AnimationComponent(Rectangle[] frames) 
        {
            OnEndAction = null;
            Action = null;
            CurrentFrame = 0;
            Frames = frames; 
        }
    }
}
