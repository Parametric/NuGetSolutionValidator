using System.Linq;
using NUnit.Framework;
using NugetSolutionValidator.DomainModels;
using NugetSolutionValidator.Validators;

namespace NugetSolutionValidator.Tests.Validators
{
    [TestFixture]
    public class NuSpecContainsOnlyRequiredDependenciesValidatorTests
    {

        [Test]
        public void Nothing_is_found_when_dependencies_match()
        {
            // Arrange
            var nuspecFile = new NuSpecFile
            {
                PackageDependencies = new[]
                        {
                            new NuGetPackageDependency{Id = "P1",Version = "v1"},
                            new NuGetPackageDependency{Id = "P2",Version = "v1"}
                        }
            };
            var projects = new[]
                {
                    new Project
                        {
                            PackageDependencies = new[]
                                {
                                    new NuGetPackageDependency {Id = "P1", Version = "v1"},
                                    new NuGetPackageDependency {Id = "P2", Version = "v1"}
                                }
                        }
                };

            var request = new NuSpecValidationRequest { NuSpecFile = nuspecFile, Projects = projects };

            var validator = new NuSpecContainsOnlyRequiredDependenciesValidator();

            // Act
            var results = validator.Validate(request).ToList();

            // Assert
            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Each_unnecessary_dependency_is_shown_then_they_do_not_match()
        {
            // Arrange
            var nuspecFile = new NuSpecFile
            {
                PackageDependencies = new[]
                        {
                            new NuGetPackageDependency{Id = "P1",Version = "v1"},
                            new NuGetPackageDependency{Id = "P2",Version = "v1"},
                            new NuGetPackageDependency{Id = "P3",Version = "v2"}
                        }
            };
            var projects = new[]
                {
                    new Project
                        {
                            PackageDependencies = new[]
                                {
                                    new NuGetPackageDependency {Id = "P1", Version = "v1"}
                                }
                        }
                };

            var request = new NuSpecValidationRequest { NuSpecFile = nuspecFile, Projects = projects };

            var validator = new NuSpecContainsOnlyRequiredDependenciesValidator();

            // Act
            var results = validator.Validate(request).ToList();

            // Assert
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results.First().Message,Is.StringContaining("P2"));
            Assert.That(results.Last().Message, Is.StringContaining("P3"));
        } 

    }
}