using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace NugetSolutionValidator
{
    public class PackageDependency
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string PackageFilePath { get; set; }

        public static readonly IEqualityComparer<PackageDependency> DefaultComparer
            = new PredicateEqualityComparer<PackageDependency>((left, right) => left.Id == right.Id && VersionEquals(left, right));

        private static bool VersionEquals(PackageDependency left, PackageDependency right)
        {
            return VersionEquals(left.Version, right.Version);
        }

        public static IEnumerable<PackageDependency> GetAllPackageDependencies(DirectoryInfo directory, bool recursive = false)
        {
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var packageFiles = directory.GetFiles("packages.config", searchOption);
            var results = packageFiles.SelectMany(GetPackageDependencies).ToArray();
            return results;
        }

        private static IEnumerable<PackageDependency> GetPackageDependencies(FileInfo packageFile)
        {
            if (packageFile == null)
                return Enumerable.Empty<PackageDependency>();

            var xDocument = XDocument.Load(packageFile.OpenRead());
            var dependencies = xDocument.Element("packages").Descendants("package")
                                        .Select(element => new PackageDependency
                                            {
                                                Id = element.Attribute("id").Value,
                                                Version = element.Attribute("version").Value,
                                                PackageFilePath = packageFile.FullName,
                                            });
            return dependencies;            
        } 

        private static bool VersionEquals(string left, string right)
        {
            if (left == right)
                return true;

            if (Regex.IsMatch(left, right))
                return true;

            if (Regex.IsMatch(right, left))
                return true;

            return false;
        }

    }
}