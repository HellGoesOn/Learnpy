﻿namespace Learnpy.Content.Components
{
    public struct TextInputComponent
    {
        public string Text;
        public bool Active;

        public TextInputComponent(string text, bool active = false)
        {
            Text = text;
            Active = active;
        }
    }
}