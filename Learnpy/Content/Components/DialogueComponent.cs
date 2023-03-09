using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Learnpy.Content.Components
{
    public struct DialogueComponent
    {
        public string[] Pages;

        public float Speed { get; set; }
        public float Progress { get; set; }
        public int DisplayedLetters { get; set; }
        public int CurrentPage { get; set; }
        public int TimeUntilNextPage { get; set; }
        public int TimeUntilNextPageMax { get; set; }
        public bool CenteredOrigin { get; set; }
        public bool Selected { get; set; }
        public bool AutoScroll { get; set; }
        public Color Color { get; set; }
        public Action OnDialogueEnd { get; set; }
        public SpriteFont Font { get; set; }

        public DialogueComponent(string[] pages)
        {
            Font = Assets.DefaultFont;
            OnDialogueEnd = null;
            TimeUntilNextPage = TimeUntilNextPageMax = 0;
            AutoScroll = false;
            Selected = false;
            Color = Color.White;
            CenteredOrigin = false;
            CurrentPage = 0;
            DisplayedLetters = 0;
            Speed = 0.1f;
            Progress = 0;
            this.Pages = pages;
        }
    }
}
