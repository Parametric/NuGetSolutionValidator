using System.Collections.Generic;

namespace NugetSolutionValidator.DomainModels
{
    public class NuSpecProjectSet
    {
        public string NuSpecFile { get; set; }

        public ICollection<string> Projects { get; set; }

        public ICollection<string> OptionalDependencies { get; set; } 
    }
}