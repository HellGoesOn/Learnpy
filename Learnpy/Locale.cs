using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Learnpy
{
    public class Locale
    {
        public static Dictionary<string, string> Translations { get; set; } = new Dictionary<string, string>();
        public static void Fill()
        {
            Translations.Clear();
            string fileText = File.ReadAllText($@"{Directory.GetCurrentDirectory()}\Content\{GameOptions.Language}\options.txt");

            string[] lines = fileText.Split(';');
            foreach (string line in lines) {
                var newLine = Regex.Replace(line, "\r\n", "");
                Translations.Add(Regex.Match(newLine, "^.*(?==)").Value, Regex.Match(newLine, "(?<==).*").Value);
            }
            
        }
    }
}
