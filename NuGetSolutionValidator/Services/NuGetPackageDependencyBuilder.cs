using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class NuGetPackageDependencyBuilder:IBuilder<ICollection<NuGetPackageDependency>>
    {
        private readonly IFileSystem _fileSystem;

        public NuGetPackageDependencyBuilder(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public ICollection<NuGetPackageDependency> Build(string packageFilePath)
        {
            if(!_fileSystem.Exists(packageFilePath))
                return new Collection<NuGetPackageDependency>();

            using (var packageFileReader = _fileSystem.OpenText(packageFilePath))
            {
                var xDocument = XDocument.Load(packageFileReader);
                var packagesSection = xDocument.Element("packages");
                var dependencies = packagesSection
                    .Descendants("package")
                    .Select(element => new NuGetPackageDependency
                        {
                            Id = element.Attribute("id").Value,
                            Version = element.Attribute("version").Value,
                        })
                    .ToList();

                return dependencies;
            }
        }
    }
}