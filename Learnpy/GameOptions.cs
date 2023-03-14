using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Learnpy
{
    public class GameOptions
    {
        internal static readonly string config = "config.cfg";

        public static int ScreenWidth { get; set; } = 1360;
        public static int ScreenHeight { get; set; } = 768;
        public static bool NeedsUpdate { get; set; }
        public static string Language { get; set; }
        public static float Volume { get; set; } = 1.0f;

        internal static Vector2 Size => new Vector2(ScreenWidth, ScreenHeight);

        public static readonly List<string> Resolutions = new List<string>() {
            "1360x768", "1920x1080", "3840x2160"
        };

        public static readonly List<float> Volumes = new List<float>() {
            0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f
        };

        public static void Save()
        {
            if (!File.Exists(config)) {
                File.Create(config).Close();

                StreamWriter sw = File.AppendText(config);
                sw.WriteLine($"ScreenWidth=1360");
                sw.WriteLine($"ScreenHeight=768");
                sw.WriteLine($"Language=en");
                sw.Write($"Volume=1.0");
                sw.Close();
            } else {
                File.WriteAllText(config, string.Empty);

                StreamWriter sw = File.AppendText(config);
                sw.WriteLine($"ScreenWidth={ScreenWidth}");
                sw.WriteLine($"ScreenHeight={ScreenHeight}");
                sw.WriteLine($"Language={Language}");
                sw.WriteLine($"Volume={Volume}");
                sw.Close();
            }
        }

        public static void Load()
        {
            if (File.Exists(config)) {
                string[] lines = File.ReadAllLines(config);
                if(lines.Length < 4) {
                    ScreenWidth = 1360;
                    ScreenHeight = 768;
                    Language = "en";
                    Volume = 1.0f;
                    return;
                }
                ScreenWidth = int.Parse(Regex.Match(lines[0], "(?<==).*").Value);
                ScreenHeight = int.Parse(Regex.Match(lines[1], "(?<==).*").Value);
                Language = Regex.Match(lines[2], "(?<==).*").Value;
                Volume = float.Parse(Regex.Match(lines[3], "(?<==).*").Value);
            }
            else {
                ScreenWidth = 1360;
                ScreenHeight = 768;
                Language = "en";
                Volume = 1.0f;
            }
        }
    }
}
