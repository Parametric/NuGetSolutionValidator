using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NugetSolutionValidator.DomainModels;
using NugetSolutionValidator.Services;
using NugetSolutionValidator.Validators;

namespace NugetSolutionValidator.NUnit
{
    [TestFixture]
    public class NuGetValidationTests
    {
        private Solution _solution;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            var solutionBuilder = new SolutionBuilder();
            var request = new BuildSolutionRequest()
                .WithSolutionName("NuGetSolutionValidator")
                .WithProjects(p=>p.Name != "NugetSolutionValidator.Tests")
                .WithNuSpec("NuGetSolutionValidator")
                .WithNuSpecProjectSet("NuGetSolutionValidator", new[] { "NuGetSolutionValidator.NUnit" });
                
            _solution = solutionBuilder.Build(request);
        }

        [Test]
        public void Show_all_project_dependencies()
        {
            // Arrange
            var dependencies = _solution
                .Projects
                .SelectMany(p => p.PackageDependencies)
                .ToList();

            // Act
            dependencies.ForEach(d=>Console.WriteLine(d.ToString()));
        }

        [Test]
        public void All_dependencies_are_same_version()
        {
            // Arrange
            var validator = new AllDependenciesAreSameVersionValidator();

            // Act
            var results = validator.Validate(_solution.Projects);

            // Assert
            Assert.That(results, Is.Empty, GetFailureMessage(results));
        }

        [Test]
        public void Nuspec_files_contain_only_one_entry_per_package_dependency()
        {
            // Arrange
            var validator = new NuSpecContainsOnlyOneEntryPerPackageValidator();

            // Act
            var results = validator.Validate(_solution.NuSpecFiles);

            // Assert
            Assert.That(results, Is.Empty, GetFailureMessage(results));

        }

        [Test]
        public void Nuspec_files_contains_required_dependencies()
        {
            // Arrange
            var validator = new NuSpecContainsAllProjectDependenciesValidator();

            // Act
            var results = validator.Validate(_solution);

            // Assert
            Assert.That(results, Is.Empty, GetFailureMessage(results));
        }

        [Test]
        public void Nuspec_files_do_not_contain_unnecessary_dependencies()
        {
            // Arrange
            var validator = new NuSpecContainsOnlyRequiredDependenciesValidator();

            // Act
            var results = validator.Validate(_solution);

            // Assert
            Assert.That(results, Is.Empty, GetFailureMessage(results));
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