using System.Collections.Generic;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class NuGetPackageDependencyBuilder:IBuilder<ICollection<NuGetPackageDependency>>
    {
        public ICollection<NuGetPackageDependency> Build(string input)
        {
            throw new System.NotImplementedException();
        }
    }
}