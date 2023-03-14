using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Core
{
    public static class SoundEngine
    {
        private static bool _isLooping;
        private static bool _updatedSong;
        private static string _currentSong;

        public static void StartMusic(string name, bool loop)
        {
            _updatedSong = true;
            _isLooping = true;
            _currentSong = name;
        }

        public static void StopMusic() 
        {
            MediaPlayer.Stop();
            _isLooping = false;
        }

        public static void Update()
        {
            if (_updatedSong) {
                _updatedSong = false;
                MediaPlayer.Stop();
                MediaPlayer.Play(Assets.GetSong(_currentSong));
            }

            if(MediaPlayer.State == MediaState.Stopped) {
                if (_isLooping) {
                    MediaPlayer.Play(Assets.GetSong(_currentSong));
                }
            }

            MediaPlayer.Volume = GameOptions.Volume;
        }

        public static SoundEffectInstance PlaySound(string name)
        {
            var fx = Assets.GetSound(name).CreateInstance();
            fx.Play();
            return fx;
        }
    }
}
