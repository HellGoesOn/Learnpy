using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Scenes.Transitions
{
    public class CombatContext : IContext
    {
        public int EnemyCount { get; set; }
        public int BulletCount { get; set; }
        public GameState ReturnsTo { get; set; } = GameState.MainMenu;
    }
}
