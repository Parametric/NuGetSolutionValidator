using System;
using System.Collections.Generic;

namespace NugetSolutionValidator.Validators
{
    public class NuSpecContainsAllProjectDependenciesValidator : IValidator<NuSpecValidationRequest>
    {
        public IEnumerable<ValidationResult> Validate(NuSpecValidationRequest toValidate)
        {
            throw new NotImplementedException();
        }
    }
}