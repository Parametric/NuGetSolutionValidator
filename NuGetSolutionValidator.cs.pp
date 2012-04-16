using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NUnit.Framework;
using System.IO;

namespace $rootnamespace$.NuGet
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
            var directory = project.Directory;
            var packageFile = directory.GetFiles("packages.config").FirstOrDefault();
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

    public class SolutionMetaData
    {
        private FileInfo[] _allProjects;
        private readonly DirectoryInfo _directory;
        private string _projectExtension = "*.csproj";

        public string SolutionName { get; private set; }

        public string ProjectExtension
        {
            get { return _projectExtension; }
            set
            {
                _projectExtension = value;
                UpdateAllProjects();
            }
        }

        public DirectoryInfo Directory
        {
            get { return _directory; }
        }

        public IEnumerable<NuSpecInfo> NuSpecFiles { get; set; }

        public IEnumerable<PackageDependency> GetAllPackageDependencies()
        {
            return NuSpecFiles.SelectMany(nsf => nsf.GetPackageDependencies());
        }

        public FileInfo[] AllProjects()
        {
            return _allProjects.ToArray();
        }

        public Func<FileInfo, bool> TestProjectsFilter { get; set; }

        public IEnumerable<FileInfo> GetTestProjects()
        {
            if (TestProjectsFilter == null)
                return Enumerable.Empty<FileInfo>();

            return AllProjects().Where(TestProjectsFilter);
        }

        public FileInfo[] GetAllProductionProjects()
        {
            return AllProjects().Except(GetTestProjects()).ToArray();
        }

        private DirectoryInfo FindSolutionRoot(string solutionFileName)
        {
            var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            var file = currentDirectory.GetFiles(solutionFileName).FirstOrDefault();
            while (file == null && currentDirectory.Parent != null)
            {
                currentDirectory = currentDirectory.Parent;
                file = currentDirectory.GetFiles(solutionFileName).FirstOrDefault();
            }
            return file == null ? null : file.Directory;
        }

        public SolutionMetaData(string solutionName)
        {
            SolutionName = solutionName;
            _directory = FindSolutionRoot(SolutionName);
            if (_directory == null)
                throw new ArgumentOutOfRangeException(string.Format("Could not find solution {0}", SolutionName));

            TestProjectsFilter = proj => proj.DirectoryName.EndsWith(".Test") || proj.DirectoryName.EndsWith(".Tests");
            UpdateAllProjects();
        }

        private void UpdateAllProjects()
        {
            _allProjects = Directory.GetFiles(this.ProjectExtension, SearchOption.AllDirectories);
        }
    }

    /// <summary>
    /// Equality comparer that accepts a function delegate.
    /// </summary>
    public class PredicateEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _predicate;

        public PredicateEqualityComparer(Func<T, T, bool> predicate)
            : base()
        {
            this._predicate = predicate;
        }

        public bool Equals(T x, T y)
        {
            if (x != null)
            {
                return ((y != null) && this._predicate(x, y));
            }

            if (y != null)
            {
                return false;
            }

            return true;
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
}