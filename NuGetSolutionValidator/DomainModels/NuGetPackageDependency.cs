namespace NugetSolutionValidator.DomainModels
{
    public class NuGetPackageDependency
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public string PackageFilePath { get; set; }

        public override string ToString()
        {
            var value = string.Format("dependency: id={0}| version={1}| packageFile={2}",
                                       this.Id,
                                       this.Version,
                                       this.PackageFilePath);

            return value;
        }

    }
}