using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Components
{
    public struct RequirementComponent
    {
        public string Description { get; set; }
        public string Solution { get; set; }
        public MissReason[] MissReasons { get; set; }
        public bool CaseSensitive { get; set; } = true;

        public RequirementComponent(string description, string solution)
        {
            Description = description;
            Solution = solution;
            MissReasons = new MissReason[0];
        }

        public string GetText(string solution)
        {
            MissReason reason;

            reason = MissReasons.FirstOrDefault(x => x.Condition.Invoke(solution));

            /*
            if (reason == null && MissReasons.Length > 0)
                reason = MissReasons[0];*/

            return reason != null ? reason.Text : "Неверный ответ!";
        }

        public bool IsMatching(string attemptedSolution)
        {
            return Solution == attemptedSolution || (!CaseSensitive && Solution.ToLower()==attemptedSolution.ToLower());
        }
    }

    public class MissReason
    {
        public string Text { get; set; }
        public Func<string, bool> Condition { get; set; }
    }
}
