using System;
using System.Collections.Generic;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class BuildSolutionRequest
    {
        public BuildSolutionRequest()
        {
            NuspecFileNames = new List<string>();
        }

        public BuildSolutionRequest(string solutionName, params string[] nuspecFileNames)
        {
            SolutionName = solutionName;
            NuspecFileNames = nuspecFileNames;
        }

        public string SolutionName { get; private set; }

        public ICollection<string> NuspecFileNames { get; private set; }

        public Func<Project, bool> ProjectFilter { get; set; }

        public BuildSolutionRequest WithSolutionName(string solutionName)
        {
            SolutionName = solutionName;

            return this;
        }

        public BuildSolutionRequest WithNuSpec(string nuspecFileName)
        {
            NuspecFileNames.Add(nuspecFileName);

            return this;
        }
    }
}