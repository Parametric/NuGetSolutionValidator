namespace NugetSolutionValidator.Services
{
    public class BuildProjectRequest
    {
        
        public string ProjectFilePath { get; set; }

        public string Name { get; set; }

        public BuildProjectRequest WithProjectFilePath(string projectFilePath)
        {
            ProjectFilePath = projectFilePath;

            return this;
        }

        public BuildProjectRequest WithName(string name)
        {
            Name = name;

            return this;
        }
    }
}