﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Learnpy
{
    public partial class Input
    {
        public static int textCursor;
        public static string editedString ="";

        public static void StartTextInput(string _editedString)
        {
            editedString = _editedString;
            TextInputEXT.TextInput += TextInputEXT_TextInput;
            TextInputEXT.StartTextInput();
        }

        private static void TextInputEXT_TextInput(char obj)
        {
            if (obj == (char)22) {
                editedString += SDL.SDL_GetClipboardText();

            } else if (obj == (char)13) {
                editedString = editedString.Insert(textCursor, Environment.NewLine);
                textCursor += Environment.NewLine.Length;

            } else if (obj == (char)8) {
                if (editedString.Length >= 1 && (textCursor >= 0 && textCursor < editedString.Length)) {
                    textCursor--;
                    editedString = editedString.Remove(textCursor, 1);
                }

                if(textCursor == editedString.Length && editedString.Length > 0) {
                    textCursor--;
                    editedString = editedString.Remove(textCursor, 1);
                }

            } else {
                editedString = editedString.Insert(textCursor, obj.ToString());
                textCursor += obj.ToString().Length;
            }
        }

        public static void StopTextInput(out string resultedString)
        {
            resultedString = editedString;
            TextInputEXT.StopTextInput();
            TextInputEXT.TextInput -= TextInputEXT_TextInput;
            editedString = "";
            textCursor = 0;
        }
    }
}
