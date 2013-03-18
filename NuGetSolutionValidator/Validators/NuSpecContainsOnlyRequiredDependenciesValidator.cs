using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NugetSolutionValidator.Validators
{
    public class NuSpecContainsOnlyRequiredDependenciesValidator : IValidator<NuSpecValidationRequest>
    {
        public IEnumerable<ValidationResult> Validate(NuSpecValidationRequest toValidate)
        {
            var optionalDependencies = toValidate.OptionalDependencies ?? new Collection<string>();

            var projectDependenciesById = toValidate
                .Projects
                .SelectMany(p => p.PackageDependencies)
                .Where(p => !optionalDependencies.Contains(p.Id))
                .ToLookup(d => d.Id);

            var unnecessaryDependencies = toValidate
                .NuSpecFile
                .PackageDependencies
                .Where(d => !projectDependenciesById.Contains(d.Id))
                .ToList();

            var validations = unnecessaryDependencies
                .Select(d => new ValidationResult
                    {
                        Message = string.Format("NuSpec file '{0}' does not contain an unnecessary dependency entry for '{1}'",
                        toValidate.NuSpecFile.Path,
                        d.Id)
                    });

            return validations;
        }
    }
}