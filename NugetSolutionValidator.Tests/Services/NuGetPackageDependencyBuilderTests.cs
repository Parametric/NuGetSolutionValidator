using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using NugetSolutionValidator.DomainModels;
using NugetSolutionValidator.Services;

namespace NugetSolutionValidator.Tests.Services
{
    [TestFixture]
    public class NuGetPackageDependencyBuilderTests
    {
        private const string _packageFilePath = "somewhere, out there";
        private ICollection<NuGetPackageDependency> _results;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            // Arrange
            var textReader = GetPackageFileContents();

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(fs => fs.Exists(_packageFilePath)).Returns(true);
            fileSystem.Setup(fs => fs.OpenText(_packageFilePath)).Returns(textReader);

            var builder = new NuGetPackageDependencyBuilder(fileSystem.Object);

            // Act
            _results = builder.Build(_packageFilePath);

        }

        private TextReader GetPackageFileContents()
        {
            return new StringReader(@"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""Moq"" version=""4.0.10827"" targetFramework=""net40"" />
  <package id=""NBuilder"" version=""3.0.1.1"" targetFramework=""net40"" />
  <package id=""NUnit"" version=""2.6.2"" targetFramework=""net40"" />
</packages>");
        }

        [Test]
        public void Then_each_dependency_is_read()
        {
            // Assert
            Assert.That(_results.Count,Is.EqualTo(3));
        }


        [Test]
        [TestCase("Moq", "4.0.10827")]
        [TestCase("NBuilder", "3.0.1.1")]
        [TestCase("NUnit", "2.6.2")]
        public void Then_each_dependency_has_its_name_and_version(string id, string version)
        {
            var dependency = _results.FirstOrDefault(r => r.Id == id);

            // Assert
            Assert.That(dependency,Is.Not.Null);
            Assert.That(dependency.Version,Is.EqualTo(version));
            Assert.That(dependency.PackageFilePath, Is.EqualTo(_packageFilePath));
        }

        [Test]
        public void When_building_and_the_file_does_not_exist_then_empty_is_returned()
        {
            // Arrange
            const string packageFilePath = "somewhere, out there";

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(fs => fs.Exists(packageFilePath)).Returns(false);

            var builder = new NuGetPackageDependencyBuilder(fileSystem.Object);

            // Act
            var results = builder.Build(packageFilePath);

            // Assert
            Assert.That(results,Is.Empty);
        }         
    }
}