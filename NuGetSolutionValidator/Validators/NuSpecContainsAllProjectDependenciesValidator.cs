using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Validators
{
    public class NuSpecContainsAllProjectDependenciesValidator : IValidator<NuSpecValidationRequest>, IValidator<Solution>
    {
        public IEnumerable<ValidationResult> Validate(NuSpecValidationRequest validationRequest)
        {
            var optionalDependencies = validationRequest.OptionalDependencies ?? new Collection<string>();

            var projectDependencies = validationRequest
                .Projects
                .SelectMany(p => p.PackageDependencies)
                .Where(p => !optionalDependencies.Contains(p.Id))
                .ToList();

            var nuspecDependenciesbyId = validationRequest
                .NuSpecFile
                .PackageDependencies
                .ToLookup(d => d.Id);

            foreach (var dependency in projectDependencies)
            {
                if (!nuspecDependenciesbyId.Contains(dependency.Id))
                {
                    var message = string.Format("NuSpec file '{0}' does not contain a dependency entry for '{1}'",
                        validationRequest.NuSpecFile.Path,
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
                                validationRequest.NuSpecFile.Path,
                                dependency.Id,
                                nuspecEntry.Version,
                                dependency.Version,
                                dependency.PackageFilePath);
                        yield return new ValidationResult { Message = message };
                    }

                  
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(Solution solution)
        {
            var results = new List<ValidationResult>();

            foreach (var nuspecSet in solution.NuSpecProjectSets)
            {
                var file = solution.NuSpecFiles.FirstOrDefault(f => string.Equals(f.Name, nuspecSet.NuSpecFile, StringComparison.CurrentCultureIgnoreCase));

                if(file == null)
                    throw new FileNotFoundException("Unable to find nuspec file",nuspecSet.NuSpecFile);

                var projectQuery = from project in solution.Projects
                               join nuspecProject in nuspecSet.Projects on project.Name.ToLower() equals nuspecProject.ToLower()
                               select project;

                var projects = projectQuery.ToList();

                if (projects.Count != nuspecSet.Projects.Count)
                {
                    var message = string.Format("Unable to find all projects defined for NuSpec file {0}.  Expected: {1} | Actual: {2}",
                                                nuspecSet.NuSpecFile,
                                                string.Join(",",nuspecSet.Projects),
                                                string.Join(",", projects.Select(p=>p.Name)));
                    throw new ApplicationException(message);
                }

                var request = new NuSpecValidationRequest
                    {
                        NuSpecFile = file,
                        Projects = projects,
                        OptionalDependencies = nuspecSet.OptionalDependencies
                    };

                results.AddRange(Validate(request));
            }

            return results;
        }
    }
}