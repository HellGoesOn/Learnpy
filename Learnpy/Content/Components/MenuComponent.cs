﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Learnpy.Content.Components
{
    public struct MenuComponent
    {
        public MenuOption[] Options { get; }

        public int SelectedIndex { get; set; }

        public bool IsSelected { get; set; }

        public SpriteFont Font { get; set; }

        public MenuComponent(params MenuOption[] options)
        {
            IsSelected = false;
            Font = Assets.DefaultFont;
            SelectedIndex = 0;
            Options = options;
            for (int i = 0; i < Options.Length; i++)
                Options[i].Index = i;
        }
    }

    public struct MenuOption
    {
        public int Index { get; set; }
        public int SelectedValue { get; set; }
        public string Name { get; set; }
        public Action Action { get; set; }
        public List<object> ValueList { get; set; }
        public object Value => ValueList[SelectedValue];
        public string LocalePath { get; set; }

        public MenuOption(string name, Action action, bool hasValues)
        {
            LocalePath = "options.txt";
            Index = 0;
            ValueList = null;
            Name = name;
            Action = action;
            SelectedValue = 0;
            if (hasValues) {
                ValueList = new List<object>();
            }
        }

        public MenuOption(string name, Action action) : this(name, action, false) { }
    }
}
