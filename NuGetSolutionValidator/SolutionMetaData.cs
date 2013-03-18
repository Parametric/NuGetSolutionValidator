using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NugetSolutionValidator
{
    public class SolutionMetaData
    {
        private string _projectExtension = "*.csproj";

        public SolutionMetaData(string solutionName)
        {
            SolutionName = solutionName;
            SolutionDirectory = FindSolutionRoot(SolutionName);
            if (SolutionDirectory == null)
                throw new ArgumentOutOfRangeException(string.Format("Could not find solution {0}", SolutionName));

            TestProjectsFilter = proj => proj.DirectoryName.EndsWith(".Test") || proj.DirectoryName.EndsWith(".Tests");
            UpdateAllProjects();
        }

        private string SolutionName { get; set; }

        private string ProjectExtension
        {
            get { return _projectExtension; }
            set
            {
                _projectExtension = value;
                UpdateAllProjects();
            }
        }

        public FileInfo[] AllProjects { get; private set; }

        public DirectoryInfo SolutionDirectory { get; private set; }

        public IEnumerable<NuSpecInfo> NuSpecFiles { get; set; }

        public IEnumerable<PackageDependency> GetAllPackageDependencies()
        {
            return PackageDependency.GetAllPackageDependencies(SolutionDirectory, recursive:true);
        }

        public Func<FileInfo, bool> TestProjectsFilter { get; set; }

        public IEnumerable<FileInfo> GetTestProjects()
        {
            if (TestProjectsFilter == null)
                return Enumerable.Empty<FileInfo>();

            return AllProjects.Where(TestProjectsFilter);
        }

        public FileInfo[] GetAllProductionProjects()
        {
            return AllProjects.Except(GetTestProjects()).ToArray();
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

        private void UpdateAllProjects()
        {
            AllProjects = SolutionDirectory.GetFiles(this.ProjectExtension, SearchOption.AllDirectories);
        }
    }
}