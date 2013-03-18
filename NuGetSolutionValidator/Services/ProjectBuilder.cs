using System.Collections.Generic;
using System.IO;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class ProjectBuilder : IBuilder<Project>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IBuilder<ICollection<NuGetPackageDependency>> _packageDependencyBuilder;

        public ProjectBuilder(IFileSystem fileSystem, IBuilder<ICollection<NuGetPackageDependency>> packageDependencyBuilder)
        {
            _fileSystem = fileSystem;
            _packageDependencyBuilder = packageDependencyBuilder;
        }

        public virtual Project Build(string projectFilePath)
        {
            var projectDirectory = _fileSystem.GetDirectory(projectFilePath);
            var packageFilePath = Path.Combine(projectDirectory, "packages.config");

            var packageDependencies = _packageDependencyBuilder.Build(packageFilePath);

            var project = new Project
                {
                    Path = projectFilePath,
                    PackageFilePath = packageFilePath,
                    PackageDependencies = packageDependencies
                };

            return project;
        }
    }
}