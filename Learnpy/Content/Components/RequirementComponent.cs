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

        public RequirementComponent(string description, string solution)
        {
            Description = description;
            Solution = solution;
        }
    }
}
