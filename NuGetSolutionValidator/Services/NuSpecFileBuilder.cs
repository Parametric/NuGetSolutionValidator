using System.Collections.Generic;
using System.IO;
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
            if (!nuspecFilePath.EndsWith(".nuspec"))
                nuspecFilePath = nuspecFilePath + ".nuspec";

            var dependencies = _packageDependencyBuilder.Build(nuspecFilePath);
            var name = Path.GetFileNameWithoutExtension(nuspecFilePath);

            return new NuSpecFile
                {
                    Name = name,
                    Path = nuspecFilePath,
                    PackageDependencies = dependencies
                };
        }
    }
}