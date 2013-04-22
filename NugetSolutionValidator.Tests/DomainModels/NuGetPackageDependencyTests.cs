using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using NUnit.Framework;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Tests.DomainModels
{
    [TestFixture]
    public class NuGetPackageDependencyTests
    {

        [Test]
        public void The_string_version_of_the_dependency_shows_its_details()
        {
            // Arrange
            var dependency = Builder<NuGetPackageDependency>.CreateNew().Build();

            // Act
            var result = dependency.ToString();

            // Assert
            Assert.That(result,Is.StringContaining(dependency.Id));
            Assert.That(result,Is.StringContaining(dependency.PackageFilePath));
            Assert.That(result,Is.StringContaining(dependency.Version));
        } 

    }
}