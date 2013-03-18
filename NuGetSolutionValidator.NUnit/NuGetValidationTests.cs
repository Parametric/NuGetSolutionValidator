using System;
using System.Linq;
using NUnit.Framework;
using NugetSolutionValidator.DomainModels;
using NugetSolutionValidator.Services;

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

            _solution = solutionBuilder.Build("NuGetSolutionValidator.sln");
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

    }
}