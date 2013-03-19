using System.Collections.Generic;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class NuSpecFileBuilder : IBuilder<NuSpecFile, string>
    {
        private readonly IBuilder<ICollection<NuGetPackageDependency>, string> _packageDependencyBuilder;

        public NuSpecFileBuilder(IBuilder<ICollection<NuGetPackageDependency>, string> nuspecDependencyBuilder)
        {
            _packageDependencyBuilder = nuspecDependencyBuilder;
        }

        public NuSpecFile Build(string nuspecFilePath)
        {
            var dependencies = _packageDependencyBuilder.Build(nuspecFilePath);

            return new NuSpecFile
                {
                    Path = nuspecFilePath,
                    PackageDependencies = dependencies
                };
        }
    }
}