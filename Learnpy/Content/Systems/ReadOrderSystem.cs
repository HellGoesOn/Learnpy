using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Learnpy.Content.Systems
{
    public class ReadOrderSystem : ISystem
    {
        // ECS violation, to be removed later
        public List<int> PuzzlePiecesInOrder = new List<int>();

        public void Execute(World gameState)
        {
            
        }

        public void Render(LearnGame gameRenderer)
        {
        }
    }
}
