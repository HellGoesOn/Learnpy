using System.Collections.Generic;

namespace Learnpy
{
    public class GameOptions
    {
        public static int ScreenWidth { get; set; } = 1360;
        public static int ScreenHeight { get; set; } = 768;
        public static bool NeedsUpdate { get; set; }

        public static readonly List<string> Resolutions = new List<string>() {
            "1360x768", "1920x1080", "3840x2160"
        };
    }
}
