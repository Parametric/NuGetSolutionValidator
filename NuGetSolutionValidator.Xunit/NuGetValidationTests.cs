using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NugetSolutionValidator.DomainModels;
using NugetSolutionValidator.Services;
using NugetSolutionValidator.Validators;
using Xunit;
using Xunit.Abstractions;

namespace NuGetSolutionValidator.Xunit
{
    
    public class NuGetValidationTests
    {
        /// <summary>
        /// See documentation at https://github.com/Parametric/NuGetSolutionValidator/
        /// </summary>

        private readonly Solution _solution;
        private readonly ITestOutputHelper _logger;

        public NuGetValidationTests(ITestOutputHelper logger)
        {
            _logger = logger;
            var solutionBuilder = new SolutionBuilder();
            var request = new BuildSolutionRequest()
                .WithSolutionName("NuGetSolutionValidator")
                .WithProjects(p => p.Name != "NugetSolutionValidator.Tests")
                .WithNuSpec("NuGetSolutionValidator.NUnit")
                .WithNuSpec("NuGetSolutionValidator.Xunit")
                .WithNuSpec("NuGetSolutionValidator.MSTest");

            _solution = solutionBuilder.Build(request);
        }

        [Fact]
        public void Show_all_project_dependencies()
        {
            // Arrange
            var dependencies = _solution
                .Projects
                .SelectMany(p => p.PackageDependencies)
                .ToList();

            // Act
            dependencies.ForEach(d => _logger.WriteLine(d.ToString()));
        }

        [Fact]
        public void All_dependencies_are_same_version()
        {
            // Arrange
            var validator = new AllDependenciesAreSameVersionValidator();

            // Act
            var results = validator.Validate(_solution);
            _logger.WriteLine(GetFailureMessage(results));
            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Nuspec_files_contain_only_one_entry_per_package_dependency()
        {
            // Arrange
            var validator = new NuSpecContainsOnlyOneEntryPerPackageValidator();

            // Act
            var results = validator.Validate(_solution.NuSpecFiles);
            _logger.WriteLine(GetFailureMessage(results));
            // Assert
            Assert.Empty(results);

        }

        [Fact]
        public void Nuspec_files_contains_required_dependencies()
        {
            // Arrange
            var validator = new NuSpecContainsAllProjectDependenciesValidator();

            // Act
            var results = validator.Validate(_solution);
            _logger.WriteLine(GetFailureMessage(results));
            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Nuspec_files_do_not_contain_unnecessary_dependencies()
        {
            // Arrange
            var validator = new NuSpecContainsOnlyRequiredDependenciesValidator();

            // Act
            var results = validator.Validate(_solution);

            _logger.WriteLine(GetFailureMessage(results));
            // Assert
            Assert.Empty(results);
        }

        private string GetFailureMessage(IEnumerable<ValidationResult> results)
        {
            var messageBuilder = new StringBuilder();

            foreach (var result in results)
            {
                messageBuilder.AppendLine(result.Message);
            }

            var message = messageBuilder.ToString();

            return message;
        }
    }
}
