using System.Collections.Generic;

namespace NugetSolutionValidator.DomainModels
{
    public class Project
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string PackageFilePath { get; set; }

        public ICollection<NuGetPackageDependency> PackageDependencies { get; set; }
    }
}