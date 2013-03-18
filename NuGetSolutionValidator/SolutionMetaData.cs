using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NugetSolutionValidator
{
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
            return PackageDependency.GetAllPackageDependencies(Directory, recursive:true);
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
}