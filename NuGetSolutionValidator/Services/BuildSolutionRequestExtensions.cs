using System;
using System.Collections.Generic;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public static class BuildSolutionRequestExtensions
    {
        public static BuildSolutionRequest WithTestProjectsExcluded(this BuildSolutionRequest request)
        {
            request.WithProjects(p => !p.Name.EndsWith(".Test") && !p.Name.EndsWith(".Tests"));
            return request;
        }

        public static BuildSolutionRequest WithProjects(this BuildSolutionRequest request, Func<Project,bool> includeExpression)
        {
            request.ProjectFilter = includeExpression;

            return request;
        }

        public static BuildSolutionRequest WithNuSpecProjectPairs(this BuildSolutionRequest request,
                                                                  string nuspecFileName,
                                                                  ICollection<string> projectNames,
                                                                  ICollection<string> optionalDependencies)
        {
            return request;
        }

    }
}