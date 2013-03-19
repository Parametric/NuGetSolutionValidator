using System.Collections.Generic;

namespace NugetSolutionValidator.Services
{
    public class BuildSolutionRequest
    {
        public BuildSolutionRequest(string solutionName, params string[] nuspecFileNames)
        {
            SolutionName = solutionName;
            NuspecFileNames = nuspecFileNames;
        }

        public string SolutionName { get; private set; }

        public ICollection<string> NuspecFileNames { get; private set; } 
    }
}