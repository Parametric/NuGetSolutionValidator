using System.Collections.Generic;
using System.Linq;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class SolutionBuilder: IBuilder<Solution>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IBuilder<Project> _projectBuilder;

        public SolutionBuilder(IFileSystem fileSystem, IBuilder<Project> projectBuilder)
        {
            _fileSystem = fileSystem;
            _projectBuilder = projectBuilder;
        }

        public virtual Solution Build(string solutionName)
        {
            var solutionFilePath = _fileSystem.FindFullFilePath(solutionName);

            var solutionFileContents = _fileSystem.ReadFile(solutionFilePath);
            var projectsInSolution = ReadProjectsFromSolution(solutionFileContents);

            var solution = new Solution
                {
                    Name = solutionName,
                    SolutionFile = solutionFilePath,
                    Projects = projectsInSolution
                };

            return solution;
        }

        private List<Project> ReadProjectsFromSolution(IEnumerable<string> solutionFileContents)
        {
            var projectPaths = GetProjectFilePathsFromSolution(solutionFileContents);
            var projectsInSolution = projectPaths
                .Select(_projectBuilder.Build)
                .ToList();
            return projectsInSolution;
        }

        private static IEnumerable<string> GetProjectFilePathsFromSolution(IEnumerable<string> solutionFileContents)
        {
            var projectLines = solutionFileContents
                .Select(line=>line.Trim())
                .Where(line => line.StartsWith("Project(\"{"));

            var projectLinesSplit = projectLines
                .Select(line => line.Split(','))
                .Where(line => line.Length > 2);

            var cleanedProjectLines = projectLinesSplit
                .Select(line => line[1])
                .Select(line=>line.Replace("\"",""))
                .Select(line=>line.TrimEnd(new[]{'"','\\'}));

            var projectDefinitions = cleanedProjectLines
                .Where(line => line.EndsWith("proj"))
                .Select(line => line.Trim());

            return projectDefinitions;
        }
    }
}