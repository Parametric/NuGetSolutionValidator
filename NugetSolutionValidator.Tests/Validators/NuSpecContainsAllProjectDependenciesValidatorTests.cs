using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NugetSolutionValidator.Tests.Validators
{
    [TestFixture]
    public class NuSpecContainsAllProjectDependenciesValidatorTests
    {

        [Test]
        public void Nothing_is_found_when_dependencies_match()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [Test]
        public void Nothing_is_found_when_dependencies_do_not_match_but_they_are_optional()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [Test]
        public void Each_missing_dependency_is_shown_then_they_do_not_match()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        } 

    }
}