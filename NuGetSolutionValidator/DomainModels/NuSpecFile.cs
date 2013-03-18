using System.Collections.Generic;

namespace NugetSolutionValidator.DomainModels
{
    public class NuSpecFile
    {
        public string Path { get; set; }

        public ICollection<NuGetPackageDependency> PackageDependencies { get; set; }
    }
}