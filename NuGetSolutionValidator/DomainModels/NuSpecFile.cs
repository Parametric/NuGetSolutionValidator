using System.Collections.Generic;

namespace NugetSolutionValidator.DomainModels
{
    public class NuSpecFile
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public ICollection<PackageDependency> PackageDependencies { get; set; }
    }
}