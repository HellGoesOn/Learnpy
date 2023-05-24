using Learnpy.Content.Components;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Learnpy.Content
{
    public class CombatContextReader
    {
        static Vector2[] enemyPositionOffsets = new Vector2[] {
            new Vector2(240, -40),
            Vector2.Zero,
            new Vector2(240, 160),
        };

        public static void ReadCombatContext(World w, string levelPath, List<Entity> enemies)
        {
            int currentEnemyCount = 0;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(File.ReadAllText(levelPath + ".txt"));

            string finalString = stringBuilder.ToString();

            string enemyList = Util.MatchBetween(finalString, "Enemies=", "[", "]"); 

            string[] fix = new string[] { Environment.NewLine }; 
            string[] enemyDescriptions = enemyList.Split(fix, StringSplitOptions.None);
            const string find_Requirement_Desc = "Desc=";
            const string find_Requirement_Answer = "Answer=";
            const string find_Texture = "Texture=";
            const string find_MissReason = "Clue=";
            const string find_CaseSensitive = "Sensitive=";
            foreach (string enemyDesc in enemyDescriptions) {

                string desc = Util.MatchBetween(enemyDesc, find_Requirement_Desc);
                string answer = Util.MatchBetween(enemyDesc, find_Requirement_Answer);
                string sensitivity = Util.MatchBetween(enemyDesc, find_CaseSensitive);
                RequirementComponent requirement = new RequirementComponent(desc, answer) {
                    CaseSensitive = sensitivity != null && sensitivity != "No"  
                };

                string texture = Util.MatchBetween(enemyDesc, find_Texture);


                var bug = w.Create();
                bug.Add(new TransformComponent(new Vector2(1000, 240) + enemyPositionOffsets[currentEnemyCount]));
                bug.Add(new TextureComponent(texture));
                bug.Add(new AnimationComponent() {
                    Action = () =>
                    {
                        bug.Get<TransformComponent>().Position += new Vector2(0, (float)Math.Sin(EntryPoint.Instance.deltaTime * 0.008f));
                    }
                });
                bug.Add(new DrawDataComponent(new Vector2(18, 21), Vector2.One * 6f) {
                    SpriteEffects = SpriteEffects.FlipHorizontally
                });
                bug.Add(requirement);
                bug.Add(new TextComponent(new TextContext(requirement.Description, Vector2.Zero) {
                    Origin = Assets.DefaultFont.MeasureString(requirement.Description) * 0.5f
                }));
                /*
                if (i == 1)
                    bug.Get<RequirementComponent>().MissReasons = missReasons;
                */
                enemies.Add(bug);

                currentEnemyCount++;
            }
        }
    }
}
