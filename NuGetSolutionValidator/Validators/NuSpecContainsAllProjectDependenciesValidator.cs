using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NugetSolutionValidator.Validators
{
    public class NuSpecContainsAllProjectDependenciesValidator : IValidator<NuSpecValidationRequest>
    {
        public IEnumerable<ValidationResult> Validate(NuSpecValidationRequest toValidate)
        {
            var optionalDependencies = toValidate.OptionalDependencies ?? new Collection<string>();

            var projectDependencies = toValidate
                .Projects
                .SelectMany(p => p.PackageDependencies)
                .Where(p => !optionalDependencies.Contains(p.Id))
                .ToList();

            var nuspecDependenciesbyId = toValidate
                .NuSpecFile
                .PackageDependencies
                .ToLookup(d => d.Id);

            foreach (var dependency in projectDependencies)
            {
                if (!nuspecDependenciesbyId.Contains(dependency.Id))
                {
                    var message = string.Format("NuSpec file '{0}' does not contain a dependency entry for '{1}'",
                        toValidate.NuSpecFile.Path,
                        dependency.Id);
                    yield return new ValidationResult {Message = message};
                }

                if (nuspecDependenciesbyId.Contains(dependency.Id)
                    && nuspecDependenciesbyId[dependency.Id].First().Version != dependency.Version)
                {
                    foreach (var nuspecEntry in nuspecDependenciesbyId[dependency.Id])
                    {
                        var message =
                            string.Format(
                                "NuSpec file '{0}'contains a dependency entry for '{1}' with version '{2}', mismatching version '{3}' in package file '{4}",
                                toValidate.NuSpecFile.Path,
                                dependency.Id,
                                nuspecEntry.Version,
                                dependency.Version,
                                dependency.PackageFilePath);
                        yield return new ValidationResult { Message = message };
                    }

                  
                }
            }
        }
    }
}