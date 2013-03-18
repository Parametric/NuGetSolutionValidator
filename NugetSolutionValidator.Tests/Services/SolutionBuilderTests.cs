using System;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using NugetSolutionValidator.DomainModels;
using NugetSolutionValidator.Services;

namespace NugetSolutionValidator.Tests.Services
{
    [TestFixture]
    public class SolutionBuilderTests
    {
        private string _solutionName;
        private Solution _solution;
        private string _fullFilePath;
        private Project _projectOne;
        private Project _projectTwo ;
        private NuSpecFile _nuspec1;
        private NuSpecFile _nuspec2;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            _solutionName = "MySolution.sln";
            _fullFilePath = @"C:\where I am\A Solution\MySolution.sln";
            _projectOne = Builder<Project>.CreateNew().Build();
            _projectTwo = Builder<Project>.CreateNew().Build();
            _nuspec1 = Builder<NuSpecFile>.CreateNew().Build();
            _nuspec2 = Builder<NuSpecFile>.CreateNew().Build();

            var solutionFileContents = GetSolutionFileContents();

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(fs => fs.FindFullFilePath(_solutionName)).Returns(_fullFilePath);
            fileSystem.Setup(fs => fs.ReadFile(_fullFilePath)).Returns(solutionFileContents);
            fileSystem.Setup(fs => fs.GetDirectory(_fullFilePath)).Returns("");

            var projectBuilder = new Mock<IBuilder<Project>>();
            projectBuilder.Setup(b => b.Build("Project1\\Project1.csproj")).Returns(_projectOne);
            projectBuilder.Setup(b => b.Build("Project2\\Project2.csproj")).Returns(_projectTwo);

            var nuspecBuilder = new Mock<IBuilder<NuSpecFile>>();
            nuspecBuilder.Setup(b => b.Build("spec1")).Returns(_nuspec1);
            nuspecBuilder.Setup(b => b.Build("spec2")).Returns(_nuspec2);

            var builder = new SolutionBuilder(fileSystem.Object, projectBuilder.Object, nuspecBuilder.Object);

            _solution = builder.Build(_solutionName,"spec1","spec2");
        }

        private string[] GetSolutionFileContents()
        {
            return @"Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""Project1"", ""Project1\Project1.csproj"", ""{E6377117-EC38-4FC7-B317-66F5EEBB11D4}""
                    EndProject
                    Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""Project2"", ""Project2\Project2.csproj"", ""{7CC0D5A6-5F89-4E3A-9EF6-C8291846C7D5}""
                    EndProject
                    Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = "".nuget"", "".nuget"", ""{209C9078-DF55-487D-9FF0-120FDAEDB22E}""
	                    ProjectSection(SolutionItems) = preProject
		                    .nuget\NuGet.exe = .nuget\NuGet.exe
		                    .nuget\NuGet.targets = .nuget\NuGet.targets
	                    EndProjectSection
                    EndProject"
                .Split(Environment.NewLine.ToCharArray());
        }

        [Test]
        public void Then_the_solution_is_is_correct()
        {
            // Assert
            Assert.That(_solution.Name,Is.EqualTo(_solutionName));
        }


        [Test]
        public void Then_the_solution_file_is_found()
        {
            // Assert
            Assert.That(_solution.SolutionFile,Is.EqualTo(_fullFilePath));
        }

        [Test]
        public void Then_the_projects_are_found()
        {
            // Assert
            Assert.That(_solution.Projects,Is.EquivalentTo(new[]{_projectOne,_projectTwo}));
        }

        [Test]
        public void Then_the_nuspec_files_are_created()
        {
            // Assert
            Assert.That(_solution.NuSpecFiles, Is.EquivalentTo(new[] { _nuspec1, _nuspec2 }));
        }

    }
}