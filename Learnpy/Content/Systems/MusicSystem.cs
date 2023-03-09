using Learnpy.Content.Components;
using Learnpy.Content.Scenes;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Systems
{
    public class MusicSystem : ISystem
    {
        public void Execute(World gameState)
        {
            Entity e = gameState.ActiveEntities.FirstOrDefault(x => x.Has<MusicComponent>());
            if (e.Id != -1) {
                MusicComponent music = e.Get<MusicComponent>();
                if (MediaPlayer.State == MediaState.Stopped) {
                    if (music.Looping) {

                        MediaPlayer.Play(Assets.GetSong(music.SongName));
                    }
                }
            }
        }

        public void Render(LearnGame gameRenderer)
        {
        }
    }
}
