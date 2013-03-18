using System.Linq;


namespace NugetSolutionValidator
{
    public class NuSpecTestConfiguration
    {
        public SolutionMetaData Solution { get; set; }

		const string SolutionName = "Your solution name here";
		
        public NuSpecTestConfiguration()
		{
            this.Solution = new SolutionMetaData(SolutionName);

            var nuspec1 = new NuSpecInfo
            {
                Name = "Your nuspec name here",
                NuSpecFile = Solution.SolutionDirectory.GetFiles("your nuspec file here").First(),
                Projects = Solution.GetAllProductionProjects().Where(p => p.Name == "your project name here").ToArray(), // provide the list of projects included in the nuspec.
                OptionalDependencies = new string[]
				{
					"OptionalPackage1",
				}, // provide a list of packages that will not be required to be included
            };

            this.Solution.NuSpecFiles = new [] {nuspec1 }; // {nuspec1, nuspec2, nuspec3, nuspec4, ...)
        }

    }
}