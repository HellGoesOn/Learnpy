using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Components
{
    public struct MusicComponent
    {
        public float Volume;
        public bool Looping;
        public string SongName;

        public MusicComponent(string songName, bool looping = true, float volume = 1.0f)
        {
            SongName = songName;
            Looping = looping;
            Volume = volume;
        }
    }
}
