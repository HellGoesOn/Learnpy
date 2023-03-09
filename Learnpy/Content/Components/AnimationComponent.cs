using Microsoft.Xna.Framework;

namespace Learnpy.Content.Components
{
    public struct AnimationComponent
    {
        public int CurrentFrame;
        public Rectangle[] Frames;

        public AnimationComponent(Rectangle[] frames) 
        {
            CurrentFrame = 0;
            Frames = frames; 
        }
    }
}
