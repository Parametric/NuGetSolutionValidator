using System.Collections.Generic;

namespace NugetSolutionValidator.Validators
{
    public interface IValidator<in T>
    {
        IEnumerable<ValidationResult> Validate(T validationRequest);
    }
}