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
            NuspecProjectSets = new Dictionary<string, NuSpecProjectSet>();
        }

        public BuildSolutionRequest(string solutionName, params string[] nuspecFileNames)
        {
            SolutionName = solutionName;
            NuspecFileNames = nuspecFileNames;
        }

        public string SolutionName { get; private set; }

        public ICollection<string> NuspecFileNames { get; private set; }

        public Func<Project, bool> ProjectFilter { get; set; }

        public IDictionary<string, NuSpecProjectSet> NuspecProjectSets { get; set; } 

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

        public BuildSolutionRequest WithNuSpecProjectSet( string nuspecFileName,
                                                                  ICollection<string> projectNames,
                                                                  ICollection<string> optionalDependencies)
        {
            var nuspecProjectSet = new NuSpecProjectSet
                {
                    NuSpecFile = nuspecFileName,
                    Projects = projectNames,
                    OptionalDependencies = optionalDependencies
                };

            NuspecProjectSets.Add(nuspecProjectSet.NuSpecFile,nuspecProjectSet);

            return this;
        }

        public BuildSolutionRequest WithProjects(Func<Project, bool> includeExpression)
        {
            ProjectFilter = includeExpression;

            return this;
        }

        public BuildSolutionRequest WithTestProjectsExcluded()
        {
            WithProjects(p => !p.Name.EndsWith(".Test") && !p.Name.EndsWith(".Tests"));

            return this;
        }
    }
}