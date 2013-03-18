using System.Collections.Generic;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Validators
{
    public class NuSpecValidationRequest
    {
        public NuSpecFile NuSpecFile { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}