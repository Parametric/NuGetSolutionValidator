using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class SolutionBuilder: IBuilder<Solution,BuildSolutionRequest>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IBuilder<Project, BuildProjectRequest> _projectBuilder;
        private readonly IBuilder<NuSpecFile, string> _nuspecFileBuilder;

        public SolutionBuilder()
            :this(new WindowsFileSystem(), 
            new ProjectBuilder(new WindowsFileSystem(), new NuGetPackageDependencyBuilder(new WindowsFileSystem())),
            new NuSpecFileBuilder(new NuSpecPackageDependencyBuilder(new WindowsFileSystem())))
        {
            
        }

        public SolutionBuilder(IFileSystem fileSystem, IBuilder<Project, BuildProjectRequest> projectBuilder, IBuilder<NuSpecFile, string> nuspecFileBuilder)
        {
            _fileSystem = fileSystem;
            _projectBuilder = projectBuilder;
            _nuspecFileBuilder = nuspecFileBuilder;
        }

        public virtual Solution Build(BuildSolutionRequest request)
        {
            var solutionName = request.SolutionName;
            
            if (!solutionName.EndsWith(".sln"))
            {
                solutionName += ".sln";
            }

            var solutionFilePath = _fileSystem.FindFullFilePath(solutionName);

            // this can return null
            if(solutionFilePath == null)
                throw new ApplicationException(string.Format("Could not locate solution file '{0}'. Looked in file path '{1}', and recursed back to the root.", Environment.CurrentDirectory, solutionName));


            var solutionFileContents = _fileSystem.ReadFile(solutionFilePath);
            var projectsInSolution = ReadProjectsFromSolution(solutionFileContents, solutionFilePath);

            var nuspecFiles = GetNuSpecFiles(request.NuspecFileNames,solutionFilePath);

            var solution = new Solution
                {
                    Name = solutionName,
                    SolutionFile = solutionFilePath,
                    Projects = projectsInSolution,
                    NuSpecFiles = nuspecFiles,
                    NuSpecProjectSets = request.NuspecProjectSets.Values,
                    ProjectFilter = request.ProjectFilter
                };

            return solution;
        }

        private ICollection<NuSpecFile> GetNuSpecFiles(ICollection<string> nuspecFileNames, string solutionFilePath)
        {
            if(nuspecFileNames == null || !nuspecFileNames.Any())
                return new Collection<NuSpecFile>();

            var solutionDirectory = _fileSystem.GetDirectory(solutionFilePath);

            var files = nuspecFileNames
                .Select(p => Path.Combine(solutionDirectory,p))
                .Select(_nuspecFileBuilder.Build)
                .ToList();

            return files;
        }

        private List<Project> ReadProjectsFromSolution(IEnumerable<string> solutionFileContents, string solutionFilePath)
        {
            var solutionDirectory = _fileSystem.GetDirectory(solutionFilePath);
            var projectPaths = GetProjectFilePathsFromSolution(solutionFileContents);
            
            var projectsInSolution = projectPaths
                .Select(p=> new Tuple<string,string>(p.Item1, Path.Combine(solutionDirectory,p.Item2)))
                .Select(p=>_projectBuilder.Build(new BuildProjectRequest().WithName(p.Item1).WithProjectFilePath(p.Item2)))
                .ToList();

            return projectsInSolution;
        }

        private static IEnumerable<Tuple<string,string>> GetProjectFilePathsFromSolution(IEnumerable<string> solutionFileContents)
        {
            var projectLines = solutionFileContents
                .Select(line=>line.Trim())
                .Where(line => line.StartsWith("Project(\"{"));

            var projectLinesSplit = projectLines
                .Select(line => line.Split(','))
                .Where(line => line.Length > 2);

            var cleanedProjectLines = projectLinesSplit
                .Select(line => new Tuple<string, string>(line[0], line[1]))
                .Select(line => new Tuple<string, string>(line.Item1.Split(new[]{'='}).LastOrDefault() ?? "", line.Item2.Replace("\"", "")))
                .Select(line => new Tuple<string, string>(line.Item1.Replace("\"", ""), line.Item2.TrimEnd(new[] { '"', '\\' })));

            var projectDefinitions = cleanedProjectLines
                .Where(line => line.Item2.EndsWith("proj"))
                 .Select(line => new Tuple<string, string>(line.Item1.Trim(), line.Item2.Trim()));

            return projectDefinitions;
        }
    }
}