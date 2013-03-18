using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class SolutionBuilder
    {
        private readonly IFileSystem _fileSystem;
        private readonly IBuilder<Project> _projectBuilder;
        private readonly IBuilder<NuSpecFile> _nuspecFileBuilder;

        public SolutionBuilder(IFileSystem fileSystem, IBuilder<Project> projectBuilder,IBuilder<NuSpecFile> nuspecFileBuilder)
        {
            _fileSystem = fileSystem;
            _projectBuilder = projectBuilder;
            _nuspecFileBuilder = nuspecFileBuilder;
        }

        public virtual Solution Build(string solutionName,params string[] nuspecFileNames)
        {
            var solutionFilePath = _fileSystem.FindFullFilePath(solutionName);

            var solutionFileContents = _fileSystem.ReadFile(solutionFilePath);
            var projectsInSolution = ReadProjectsFromSolution(solutionFileContents);

            var nuspecFiles = GetNuSpecFiles(nuspecFileNames);

            var solution = new Solution
                {
                    Name = solutionName,
                    SolutionFile = solutionFilePath,
                    Projects = projectsInSolution,
                    NuSpecFiles = nuspecFiles
                };

            return solution;
        }

        private ICollection<NuSpecFile> GetNuSpecFiles(ICollection<string> nuspecFileNames)
        {
            if(nuspecFileNames == null || !nuspecFileNames.Any())
                return new Collection<NuSpecFile>();

            var files = nuspecFileNames
                .Select(_nuspecFileBuilder.Build)
                .ToList();

            return files;
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