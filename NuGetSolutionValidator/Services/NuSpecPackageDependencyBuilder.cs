using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NugetSolutionValidator.DomainModels;

namespace NugetSolutionValidator.Services
{
    public class NuSpecPackageDependencyBuilder:IBuilder<ICollection<NuGetPackageDependency>>
    {
        private readonly IFileSystem _fileSystem;

        public NuSpecPackageDependencyBuilder(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public ICollection<NuGetPackageDependency> Build(string nuspecFilePath)
        {
            if (!_fileSystem.Exists(nuspecFilePath))
                throw new FileNotFoundException("Unable to find nuspec file", nuspecFilePath);

            using (var packageFileReader = _fileSystem.OpenText(nuspecFilePath))
            {
                var xDocument = XDocument.Load(packageFileReader);
                var dependencies = xDocument
                    .Element("package")
                    .Element("metadata")
                    .Element("dependencies")
                    .Descendants("dependency")
                    .Select(element => new NuGetPackageDependency
                        {
                            Id = element.Attribute("id").Value,
                            Version = element.Attribute("version").Value,
                            PackageFilePath = nuspecFilePath
                        })
                    .ToList();

                return dependencies;
            }
        }
    }
}