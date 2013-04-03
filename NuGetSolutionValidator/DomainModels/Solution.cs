using System;
using System.Collections.Generic;
using System.IO;

namespace NugetSolutionValidator.DomainModels
{
    public class Solution
    {
        public Solution()
        {
            NuSpecProjectSets = new List<NuSpecProjectSet>();
        }

        public string Name { get; set; }
 
        public string SolutionFile { get; set; }

        public ICollection<Project> Projects { get; set; }

        public ICollection<NuSpecFile> NuSpecFiles { get; set; }

        public ICollection<NuSpecProjectSet> NuSpecProjectSets { get; set; }

        public Func<Project, bool> ProjectFilter { get; set; }
    }
}