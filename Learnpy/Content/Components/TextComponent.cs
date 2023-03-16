using System;

namespace Learnpy.Content.Components
{
    public struct TextComponent
    {
        public TextContext[] Texts;
        public Action Action { get; set; }

        public TextComponent(TextContext[] texts)
        {
            this.Texts = texts;
            Action = null;
        }

        public TextComponent(TextContext text)
        {
            this.Texts = new[] { text };
            Action = null;
        }

        public ref TextContext Get(int i) => ref Texts[i];
    }
}
