using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NugetSolutionValidator.DomainModels;
using NugetSolutionValidator.Services;
using NugetSolutionValidator.Validators;

namespace NuGetSolutionValidator.MsTest
{
    [TestClass]
    public class NuGetValidationTests
    {
        private static Solution _solution;

        /// <summary>
        /// See documentation at https://github.com/Parametric/NuGetSolutionValidator/
        /// </summary>
        
        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            var solutionBuilder = new SolutionBuilder();
            var request = new BuildSolutionRequest()
                .WithSolutionName("NuGetSolutionValidator")
                .WithProjects(p => p.Name != "NugetSolutionValidator.Tests")
                .WithNuSpec("NuGetSolutionValidator.NUnit")
                .WithNuSpec("NuGetSolutionValidator.MSTest");

            _solution = solutionBuilder.Build(request);
        }

        [TestMethod]
        public void Show_all_project_dependencies()
        {
            // Arrange
            var dependencies = _solution
                .Projects
                .SelectMany(p => p.PackageDependencies)
                .ToList();

            // Act
            dependencies.ForEach(d => Console.WriteLine(d.ToString()));
        }

        [TestMethod]
        public void All_dependencies_are_same_version()
        {
            // Arrange
            var validator = new AllDependenciesAreSameVersionValidator();

            // Act
            var results = validator.Validate(_solution).ToList();

            // Assert
            Assert.AreEqual(results.Count, 0, GetFailureMessage(results));
        }

        [TestMethod]
        public void Nuspec_files_contain_only_one_entry_per_package_dependency()
        {
            // Arrange
            var validator = new NuSpecContainsOnlyOneEntryPerPackageValidator();

            // Act
            var results = validator.Validate(_solution.NuSpecFiles).ToList();

            // Assert
            Assert.AreEqual(results.Count, 0, GetFailureMessage(results));

        }

        [TestMethod]
        public void Nuspec_files_contains_required_dependencies()
        {
            // Arrange
            var validator = new NuSpecContainsAllProjectDependenciesValidator();

            // Act
            var results = validator.Validate(_solution).ToList();

            // Assert
            Assert.AreEqual(results.Count, 0, GetFailureMessage(results));
        }

        [TestMethod]
        public void Nuspec_files_do_not_contain_unnecessary_dependencies()
        {
            // Arrange
            var validator = new NuSpecContainsOnlyRequiredDependenciesValidator();

            // Act
            var results = validator.Validate(_solution).ToList();

            // Assert
            Assert.AreEqual(results.Count, 0, GetFailureMessage(results));
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
