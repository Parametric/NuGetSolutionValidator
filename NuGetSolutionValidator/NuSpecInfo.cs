using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NugetSolutionValidator
{
    public class NuSpecInfo
    {
        public FileInfo NuSpecFile { get; set; }
        public string[] OptionalDependencies { get; set; }
        public FileInfo[] Projects { get; set; }

        public string Name { get; set; }

        public PackageDependency[] RequiredDependencies { get; set; }

        public IEnumerable<PackageDependency> GetPackageDependencies()
        {
            return Projects
                .SelectMany(GetPackageDependencies)
                .OrderBy(p => p.Id)
                .ToArray();
        }

        public IEnumerable<PackageDependency> GetPackageDependencies(FileInfo project)
        {
            return PackageDependency.GetAllPackageDependencies(project.Directory);
        }

        public IEnumerable<PackageDependency> GetUnregisteredDependencies()
        {
            var expectedDepedencies = GetExpectedPackageDependencies();
            var actualDependencies = GetNuSpecDependencies();
            var unregisteredDependencies = expectedDepedencies
                .Except(actualDependencies, PackageDependency.DefaultComparer)
                .ToArray();
            return unregisteredDependencies;
        }

        public IEnumerable<PackageDependency> GetNuSpecDependencies()
        {
            if (NuSpecFile == null)
                throw new NullReferenceException("NuSpecFile is reqired.");

            if (!NuSpecFile.Exists)
                throw new FileNotFoundException(string.Format("No nuspec file found at {0}", NuSpecFile.FullName));

            var xDocument = XDocument.Load(NuSpecFile.OpenRead());
            var dependencies = xDocument
                .Element("package")
                .Element("metadata")
                .Element("dependencies")
                .Descendants("dependency")
                .Select(element => new PackageDependency
                    {
                        Id = element.Attribute("id").Value,
                        Version = element.Attribute("version").Value,
                        PackageFilePath = NuSpecFile.FullName,
                    });
            return dependencies;
        }

        public IEnumerable<PackageDependency> GetExpectedPackageDependencies()
        {
            var packageDependencies = GetPackageDependencies();
            if (RequiredDependencies != null)
                packageDependencies = packageDependencies.Concat(RequiredDependencies);

            return packageDependencies
                .Where(IsOptional)
                .Distinct(PackageDependency.DefaultComparer)
                .OrderBy(p => p.Id)
                .ToArray();
        }

        private bool IsOptional(PackageDependency ed)
        {
            return OptionalDependencies == null || !OptionalDependencies.Contains(ed.Id);
        }

        public IEnumerable<PackageDependency> GetUnnecessarilyRegisteredNuspecDependencies()
        {
            var packageDependencies = GetExpectedPackageDependencies();
            return GetNuSpecDependencies()
                .Except(packageDependencies, PackageDependency.DefaultComparer)
                .Distinct(PackageDependency.DefaultComparer)
                .OrderBy(p => p.Id)
                .ToArray();
        }

    }
}