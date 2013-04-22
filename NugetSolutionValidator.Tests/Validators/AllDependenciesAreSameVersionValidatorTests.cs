using System.Linq;
using NUnit.Framework;
using NugetSolutionValidator.DomainModels;
using NugetSolutionValidator.Validators;

namespace NugetSolutionValidator.Tests.Validators
{
    [TestFixture]
    public class AllDependenciesAreSameVersionValidatorTests
    {
        [Test]
        public void When_all_packages_are_the_same_version_then_nothing_is_found()
        {
            // Arrange
            var solution = new Solution
                {
                    Projects = new[]
                        {
                            new Project{
                                PackageDependencies = new[]
                                {
                                    new NuGetPackageDependency{Id="MyPackage",Version = "1.0",PackageFilePath = "where I am 1"},
                                    new NuGetPackageDependency{Id="Something Else",Version = "2.0",PackageFilePath = "where I am 1"},
                                }
                            },
                            new Project{
                                PackageDependencies = new[]
                                {
                                    new NuGetPackageDependency{Id="MyPackage",Version = "1.0",PackageFilePath = "where I am 2"},
                                    new NuGetPackageDependency{Id="Yet Another Thing",Version = "3.0",PackageFilePath = "where I am 2"},
                                }
                            },
                        }
                };

            var validator = new AllDependenciesAreSameVersionValidator();

            // Act
            var results = validator.Validate(solution.Projects);

            // Assert
            Assert.That(results,Is.Empty);
        }

        [Test]
        public void When_packages_have_different_versions_then_each_one_is_returned()
        {
            // Arrange
            var solution = new Solution
            {
                Projects = new[]
                        {
                            new Project{
                                PackageDependencies = new[]
                                {
                                    new NuGetPackageDependency{Id="MyPackage",Version = "1.0",PackageFilePath = "where I am 1"},
                                    new NuGetPackageDependency{Id="Something Else",Version = "2.0",PackageFilePath = "where I am 1"},
                                }
                            },
                            new Project{
                                PackageDependencies = new[]
                                {
                                    new NuGetPackageDependency{Id="MyPackage",Version = "1.1",PackageFilePath = "where I am 2"},
                                    new NuGetPackageDependency{Id="Yet Another Thing",Version = "3.0",PackageFilePath = "where I am 2"},
                                }
                            },
                        }
            };

            var validator = new AllDependenciesAreSameVersionValidator();

            // Act
            var results = validator.Validate(solution.Projects).ToList();

            // Assert
            Assert.That(results.Count(),Is.EqualTo(1));
            Assert.That(results[0].Message,Is.StringContaining("MyPackage"));
            Assert.That(results[0].Message,Is.StringContaining("1.0"));
            Assert.That(results[0].Message,Is.StringContaining("1.1"));
            Assert.That(results[0].Message,Is.StringContaining("where I am 1"));
            Assert.That(results[0].Message,Is.StringContaining("where I am 2"));
        }
    }
}