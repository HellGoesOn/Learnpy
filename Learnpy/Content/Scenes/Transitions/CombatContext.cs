using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Scenes.Transitions
{
    public class CombatContext : IContext
    {
        public CombatContext()
        {
            EndCondition = () => {
                return EnemyCount <= 0;
                };

            FailCondition = () =>
            {
                return BulletCount <= 0 && EnemyCount > 0;
            };
        }

        public string LessonPath { get; set; } = "ShootOut1";

        public int EnemyCount { get; set; }
        public int BulletCount { get; set; }

        public Func<bool> EndCondition { get; set; }
        public Func<bool> FailCondition { get; set; }
        public GameState ReturnsTo { get; set; } = GameState.CombatSelect;
        public GameState ProceedsTo { get; set; } = GameState.CombatSelect;

        public Func<bool> GetSuccessCondition() => EndCondition;
        public Func<bool> GetFailCondition() => FailCondition;
    }
}
