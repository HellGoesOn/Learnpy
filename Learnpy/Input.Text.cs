using Microsoft.Xna.Framework;
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

        private static bool _alreadyActive;

        public static void StartTextInput(string _editedString)
        {
            if (!_alreadyActive) {
                TextInputEXT.TextInput += TextInputEXT_TextInput;
                TextInputEXT.StartTextInput();
            }
            editedString = _editedString;
            textCursor = _editedString.Length;
            _alreadyActive = true;
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
                    if (textCursor < 0)
                        textCursor = 0;
                    editedString = editedString.Remove(textCursor, 1);
                }

                if(textCursor == editedString.Length && editedString.Length > 0) {
                    textCursor--;
                    editedString = editedString.Remove(textCursor, 1);
                }

            } else if(obj != (char)127){
                editedString = editedString.Insert(textCursor, obj.ToString());
                textCursor += obj.ToString().Length;
            }
        }

        public static void StopTextInput(out string resultedString)
        {
            _alreadyActive = false;
            resultedString = editedString;
            TextInputEXT.StopTextInput();
            TextInputEXT.TextInput -= TextInputEXT_TextInput;
            editedString = "";
            textCursor = 0;
        }
    }
}
