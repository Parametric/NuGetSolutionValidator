using System.Collections.Generic;

namespace NugetSolutionValidator.Validators
{
    public class NuSpecContainsOnlyRequiredDependenciesValidator : IValidator<NuSpecValidationRequest>
    {
        public IEnumerable<ValidationResult> Validate(NuSpecValidationRequest toValidate)
        {
            throw new System.NotImplementedException();
        }
    }
}