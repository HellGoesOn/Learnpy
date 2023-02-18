using System;
using System.Collections.Generic;

namespace Learnpy.Content.Components
{
    public struct MenuComponent
    {
        public MenuOption[] Options { get; }

        public int SelectedIndex { get; set; }

        public bool IsSelected { get; set; }

        public MenuComponent(params MenuOption[] options)
        {
            IsSelected = false;
            SelectedIndex = 0;
            Options = options;
        }
    }

    public struct MenuOption
    {
        public int SelectedValue { get; set; }
        public string Name { get; set; }
        public Action Action { get; set; }
        public List<object> ValueList { get; set; }
        public object Value => ValueList[SelectedValue];

        public MenuOption(string name, Action action, bool hasValues)
        {
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
