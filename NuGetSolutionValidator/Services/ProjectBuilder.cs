using System.Collections.Generic;
using System.IO;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class ProjectBuilder : IBuilder<Project, BuildProjectRequest>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IBuilder<ICollection<NuGetPackageDependency>, string> _packageDependencyBuilder;

        public ProjectBuilder(IFileSystem fileSystem, IBuilder<ICollection<NuGetPackageDependency>, string> packageDependencyBuilder)
        {
            _fileSystem = fileSystem;
            _packageDependencyBuilder = packageDependencyBuilder;
        }

        public virtual Project Build(BuildProjectRequest request)
        {
            var projectDirectory = _fileSystem.GetDirectory(request.ProjectFilePath);
            var packageFilePath = Path.Combine(projectDirectory, "packages.config");

            var packageDependencies = _packageDependencyBuilder.Build(packageFilePath);

            var project = new Project
                {
                    Name = request.Name,
                    Path = request.ProjectFilePath,
                    PackageFilePath = packageFilePath,
                    PackageDependencies = packageDependencies
                };

            return project;
        }
    }
}