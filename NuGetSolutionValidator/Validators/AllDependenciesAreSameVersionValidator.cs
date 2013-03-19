using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Validators
{
    public class AllDependenciesAreSameVersionValidator : IValidator<ICollection<Project>>
    {
        public IEnumerable<ValidationResult> Validate(ICollection<Project> toValidate)
        {
            var packagesById = toValidate
                                        .SelectMany(p => p.PackageDependencies)
                                        .GroupBy(p => p.Id);

            var multiplePackages = packagesById
                .Where(g => g.Select(p => p.Version).Distinct().Count() > 1);

            var results = multiplePackages.Select(GetValidationResult);

            return results;

        }

        private ValidationResult GetValidationResult(IEnumerable<NuGetPackageDependency> dependency)
        {
            var realizedDependencies = dependency.ToList();

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendFormat("Multiple versions found for package '{0}':{1}", realizedDependencies.First().Id,Environment.NewLine);
            realizedDependencies.ForEach(d=>messageBuilder.AppendFormat("  ({0}) {1}{2}",d.Version,d.PackageFilePath,Environment.NewLine));
            var message = messageBuilder.ToString();

            var result = new ValidationResult {Message = message};

            return result;
        }
    }
}