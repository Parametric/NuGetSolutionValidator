using System;
using System.Linq;
using NUnit.Framework;

namespace $rootnamespace$.NuGet
{
    [TestFixture]
    public class NuSpecTests
    {
        protected SolutionMetaData Solution { get; set; }

		const string SolutionName = "Your solution name here";
		
        [TestFixtureSetUp]
        public void BeforeAnyTestRuns()
		{
		
            this.Solution = new SolutionMetaData(SolutionName);

            var nuspec1 = new NuSpecInfo
            {
                Name = "Your nuspec name here",
                NuSpecFile = Solution.Directory.GetFiles("your nuspec file here").First(),
                Projects = Solution.GetAllProductionProjects().Where(p => p.Name == "your project name here").ToArray(), // provide the list of projects included in the nuspec.
                OptionalDependencies = new string[]
				{
					"OptionalPackage1",
				}, // provide a list of packages that will not be required to be included
            };

            this.Solution.NuSpecFiles = new [] {nuspec1 }; // {nuspec1, nuspec2, nuspec3, nuspec4, ...)
        }

        [Test]
        public void NuSpecsAreWellDefined()
        {
            foreach (var nuspec in this.Solution.NuSpecFiles)
            {
                Assert.That(nuspec.Projects.Any(), "{0} does not have any projects registered.", nuspec.Name);
                Assert.That(nuspec.NuSpecFile, Is.Not.Null, "{0} does not have a configured nuspec file.", nuspec.Name);
                Assert.That(nuspec.NuSpecFile.Exists, "{0} does not have a valid nuspec file. {1} does not exist.", nuspec.Name, nuspec.NuSpecFile.FullName);
            }
        }

        [Test]
        public void DisplayPackageDependencies()
        {
			// not really a test, but useful for seeing the "whole picture" when one of the other tests fails.
			
            foreach (var dependency in Solution.GetAllPackageDependencies())
                Write(dependency);

        }

        [Test]
        public void AllDependenciesForTheSamePackageAreForTheSameVersion()
        {

            // Act
            var packageVersions = from dependency in Solution.GetAllPackageDependencies()
                                  group dependency by dependency.Id
                                    into packageGroup
                                    let versions = packageGroup.Select(pg => pg.Version).Distinct().ToArray()
                                    select new
                                    {
                                        Id = packageGroup.Key,
                                        Versions = versions,
                                        Count = versions.Length,
                                    };

            // Assert

            foreach (var packageVersion in packageVersions)
            {
                Assert.That(packageVersion.Count, Is.EqualTo(1), "Package {0} references more than one version in the solution: {1}", 
                    packageVersion.Id, string.Join(", ", packageVersion.Versions));
            }

        }

        [Test]
        public void AllDependenciesAreRegisteredInNuSpecs()
        {
            foreach (var nuspec in Solution.NuSpecFiles)
            {
                Console.WriteLine("Expected dependency registration list for {0}", nuspec.NuSpecFile.FullName);
                var expectedPackageDependencies = nuspec.GetExpectedPackageDependencies();
                foreach (var dependency in expectedPackageDependencies)
                {
                    Console.WriteLine(@"<dependency id=""{0}"" version=""{1}"" />", dependency.Id, dependency.Version);
                }

                Console.WriteLine("");
                Console.WriteLine("Unregistered dependencies in {0}", nuspec.NuSpecFile.FullName);

                var unregisteredDependencies = nuspec.GetUnregisteredDependencies();
                foreach (var dependency in unregisteredDependencies)
                    Write(dependency);

                Assert.That(unregisteredDependencies, Is.Empty, "");

                Console.WriteLine("");
            }

        }

        [Test]
        public void ThereAreNoUnnecessaryDependenciesAreRegisteredInNuspecs()
        {
            foreach (var nuspec in Solution.NuSpecFiles)
            {
                Console.WriteLine("Expected dependency registration list for {0}", nuspec.NuSpecFile.FullName);
                var expectedPackageDependencies = nuspec.GetExpectedPackageDependencies();
                foreach (var dependency in expectedPackageDependencies)
                {
                    Console.WriteLine(@"<dependency id=""{0}"" version=""{1}"" />", dependency.Id, dependency.Version);
                }

                Console.WriteLine("");
                Console.WriteLine("Unnecessary dependencies in {0}", nuspec.NuSpecFile.FullName);

                var unregisteredDependencies = nuspec.GetUnnecessarilyRegisteredNuspecDependencies();
                foreach (var dependency in unregisteredDependencies)
                    Write(dependency);

                Assert.That(unregisteredDependencies, Is.Empty, "");

                Console.WriteLine("");
            }
        }

        private static void Write(PackageDependency dependency)
        {
            Console.WriteLine(string.Format("dependency: id={0}, version={1}, packageFile={2}",
                                            dependency.Id, dependency.Version, dependency.PackageFilePath));
        }
    }
}