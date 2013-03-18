using System.Collections.Generic;
using System.Linq;
using System.Text;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Validators
{
    public class NuSpecContainsOnlyOneEntryPerPackageValidator : IValidator<ICollection<NuSpecFile>>
    {
        public IEnumerable<ValidationResult> Validate(ICollection<NuSpecFile> toValidate)
        {
            foreach (var nuSpecFile in toValidate)
            {
                var packagesById = nuSpecFile.PackageDependencies
                                        .GroupBy(p => p.Id);

                var multiplePackages = packagesById
                    .Where(g => g.Select(p => p.Version).Distinct().Count() > 1);

                foreach (var package in multiplePackages)
                {
                    yield return GetValidationResult(nuSpecFile, package);
                }
            }
           
        }

        private ValidationResult GetValidationResult(NuSpecFile file, IEnumerable<NuGetPackageDependency> dependency)
        {
            var realizedDependencies = dependency.ToList();

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendFormat("NuSpec file '{0}' Multiple versions found for package '{1}':", file.Path, realizedDependencies.First().Id);
            realizedDependencies.ForEach(d => messageBuilder.AppendFormat("| ({0}) {1}", d.Version, d.PackageFilePath));
            var message = messageBuilder.ToString();

            var result = new ValidationResult { Message = message };

            return result;
        }
    }
}