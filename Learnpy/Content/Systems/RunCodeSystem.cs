using Learnpy.Content.Components;
using Learnpy.Core;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Learnpy.Content.Systems
{
    public class RunCodeSystem : ISystem
    {
        /// <summary>
        ///  ECS violation. Needs to be removed later
        /// </summary>
        public string CurrentSentence;

        public List<int> PuzzlePiecesInOrder = new List<int>();

        public void Execute(World gameState)
        {
            if(Input.PressedKey(Keys.R))
            {
                string sentence = "";
                foreach(int id in PuzzlePiecesInOrder)
                {
                    var e = gameState.Entities[id];

                    if(e.HasComponent<PuzzleComponent>())
                    {
                        sentence += e.GetComponent<PuzzleComponent>().StoredText + " ";
                    }
                }
                CurrentSentence = sentence;
            }

            if (Input.PressedKey(Keys.Q))
            {
                GetOrderList(gameState);
            }
        }

        private void GetOrderList(World gameState)
        {
            PuzzlePiecesInOrder.Clear();
            foreach (int id in gameState.EntitiesById)
            {
                var e = gameState.Entities[id];

                if (e.HasComponent<PuzzleComponent>())
                {
                    if (e.HasComponent<MoveableComponent>())
                        continue;

                    var puzzle = e.GetComponent<PuzzleComponent>();

                    List<int> ids = new List<int>();

                    if (puzzle.ConnectionTo != -1)
                    {
                        var otherEntity = gameState.Entities[puzzle.ConnectionTo];
                        ids.Add(otherEntity.Id);
                        var otherPuzzle = otherEntity.GetComponent<PuzzleComponent>();

                        while (otherEntity.GetComponent<PuzzleComponent>().ConnectionTo != -1)
                        {
                            otherPuzzle = otherEntity.GetComponent<PuzzleComponent>();
                            otherEntity = gameState.Entities[otherPuzzle.ConnectionTo];
                            ids.Add(otherEntity.Id);
                        }
                    }

                    foreach (var i in ids)
                    {
                        PuzzlePiecesInOrder.Add(i);
                    }
                }
            }
        }

        public void Render(LearnGame gameRenderer)
        {
            if(!string.IsNullOrEmpty(CurrentSentence))
                gameRenderer.spriteBatch.DrawString(Assets.DefaultFont, CurrentSentence, new Vector2(20, 460), Color.Purple, 0f, Vector2.Zero, Vector2.One, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1f);
        }
    }
}
