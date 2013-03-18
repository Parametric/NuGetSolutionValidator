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
    public class NuSpecPackageDependencyBuilderTests
    {
        private const string _nuspecFilePath = "somewhere, out there";
        private ICollection<NuGetPackageDependency> _results;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            // Arrange
            var textReader = GetNuSpecFileContents();

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(fs => fs.Exists(_nuspecFilePath)).Returns(true);
            fileSystem.Setup(fs => fs.OpenText(_nuspecFilePath)).Returns(textReader);

            var builder = new NuSpecPackageDependencyBuilder(fileSystem.Object);

            // Act
            _results = builder.Build(_nuspecFilePath);

        }

        private TextReader GetNuSpecFileContents()
        {
            return new StringReader(@"<?xml version=""1.0""?>
<package >
  <metadata>
    <id>Some.Pckage</id>
    <version>$version$</version>
    <authors>I wrote this</authors>
    <owners>I own this</owners>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <description>Does things</description>
    <copyright>Copyright 1971</copyright>
    <tags>Test</tags>
    <dependencies>
      <dependency id=""AutoMapper"" version=""2.0.0"" />
      <dependency id=""Castle.Core"" version=""2.5.2"" />
      <dependency id=""EntityFramework"" version=""4.1.10715.0"" />
    </dependencies>
  </metadata>
  <files>
    <file src=""SomeProject\bin\Release\SomeProject.dll"" target=""lib/net40"" />
    <file src=""SomeProject.Web\bin\SomeProject.Web.dll"" target=""lib/net40"" />
  </files>
</package>");
        }

        [Test]
        public void Then_each_dependency_is_read()
        {
            // Assert
            Assert.That(_results.Count,Is.EqualTo(3));
        }


        [Test]
        [TestCase("AutoMapper", "2.0.0")]
        [TestCase("Castle.Core", "2.5.2")]
        [TestCase("EntityFramework", "4.1.10715.0")]
        public void Then_each_dependency_has_its_name_and_version(string id, string version)
        {
            var dependency = _results.FirstOrDefault(r => r.Id == id);

            // Assert
            Assert.That(dependency,Is.Not.Null);
            Assert.That(dependency.Version,Is.EqualTo(version));
            Assert.That(dependency.PackageFilePath, Is.EqualTo(_nuspecFilePath));
        }

        [Test]
        public void When_building_and_the_file_does_not_exist_then_an_exception_is_thrown()
        {
            // Arrange
            const string packageFilePath = "somewhere, out there";

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(fs => fs.Exists(packageFilePath)).Returns(false);

            var builder = new NuSpecPackageDependencyBuilder(fileSystem.Object);

            // Act
            var result = Assert.Throws<FileNotFoundException>(()=>builder.Build(packageFilePath));

            // Assert
            Assert.That(result, Is.Not.Null);
        }         
    }
}