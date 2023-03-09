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

        public static string GetTranslation(string key, string source = "options.txt")
        {
            if (source == "options.txt") {
                if (Translations.TryGetValue(key, out var result))
                    return result;
                else
                    return "???";
            } else {
                string path = $@"{Directory.GetCurrentDirectory()}\Content\{GameOptions.Language}\{source}";

                if (!File.Exists(path))
                    return "???";

                string fileText = File.ReadAllText($@"{Directory.GetCurrentDirectory()}\Content\{GameOptions.Language}\{source}");

                string[] lines = fileText.Replace("\r\n", "").Split(';');
                foreach (string line in lines) {
                    Match match = Regex.Match(line, "(?<=" + key +"=).*");
                    if (match.Success)
                        return match.Value.Replace("<br>", Environment.NewLine);
                }
                return "???";
            }
        }
    }
}
