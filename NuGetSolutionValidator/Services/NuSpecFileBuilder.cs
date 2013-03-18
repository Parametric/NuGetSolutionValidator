using System.Collections.Generic;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class NuSpecFileBuilder:IBuilder<NuSpecFile>
    {
        private readonly IBuilder<ICollection<NuGetPackageDependency>> _packageDependencyBuilder;

        public NuSpecFileBuilder(IBuilder<ICollection<NuGetPackageDependency>> nuspecDependencyBuilder)
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