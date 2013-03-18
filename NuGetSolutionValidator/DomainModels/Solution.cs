using System.Collections.Generic;

namespace NugetSolutionValidator.DomainModels
{
    public class Solution
    {
        public string Name { get; set; }
 
        public string Path { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}