namespace NugetSolutionValidator.Services
{
    public interface IBuilder<out T>
    {
        T Build(string input);
    }
}